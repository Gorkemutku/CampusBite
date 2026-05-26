using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IdentityDbContext _context;

        public UsersController(IdentityDbContext context)
        {
            _context = context;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto request)
        {
            // 1. E-posta adresinden domain'i (uzantıyı) ayırıyoruz
            var emailParts = request.Email.Split('@');
            if (emailParts.Length != 2)
            {
                return BadRequest("Geçersiz e-posta formatı.");
            }
            
            var domain = emailParts[1];

            // 2. Veritabanına gidip bu uzantıya sahip aktif bir üniversite var mı diye bakıyoruz
            var university = await _context.Universities
                .FirstOrDefaultAsync(u => u.Domain == domain && u.IsActive);

            // Eğer eşleşme yoksa, sistemi dışarıya kapatıyoruz
            if (university == null)
            {
                return BadRequest("Üniversiteniz henüz CampusBite sistemine kayıtlı değil veya edu.tr uzantılı geçerli bir e-posta girmediniz.");
            }

            // 3. Eşleşme başarılıysa kullanıcıyı oluşturuyor ve üniversitesini otomatik bağlıyoruz
            var newUser = new User
            {
                Email = request.Email,
                PasswordHash = request.Password, // Not: İleride güvenliği artırmak için bunu hash'leyeceğiz (BCrypt vs.)
                FirstName = request.FirstName,
                LastName = request.LastName,
                UniversityId = university.Id, // Sistem arka planda ID'yi eşleştirdi
                IsEmailConfirmed = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                Message = "Kayıt başarılı! E-posta onayından sonra giriş yapabilirsiniz.", 
                UserId = newUser.Id, 
                UniversityName = university.Name 
            });
        }
        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(UserLoginDto request)
        {
            // 1. Kullanıcıyı e-posta ve şifresiyle veritabanında arıyoruz
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null)
            {
                return Unauthorized("E-posta veya şifre hatalı.");
            }

            // 2. Kullanıcı bulunduysa ona özel JWT (Dijital Kimlik Kartı) oluşturuyoruz
            var tokenHandler = new JwtSecurityTokenHandler();
            // appsettings.json'daki gizli anahtarımızı alıyoruz (Gerçek projede daha güvenli alınır, şimdilik basit tutuyoruz)
            var key = Encoding.ASCII.GetBytes("CampusBite_Cok_Gizli_Ve_Uzun_Bir_Sifre_Anahtari_2026!"); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Kimlik kartının içine basılacak bilgiler (Claims)
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UniversityId", user.UniversityId.ToString()) // Hangi üniversitede olduğu bilgisi!
                }),
                Expires = DateTime.UtcNow.AddMinutes(120), // 2 saat geçerli
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "CampusBiteIdentityServer",
                Audience = "CampusBiteMicroservices"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            // 3. Oluşturulan token'ı kullanıcıya geri veriyoruz
            return Ok(new { Token = jwtString, Message = "Giriş başarılı!" });
        }
    }

    // Dışarıdan sadece bu bilgileri kabul edeceğimizi belirten DTO sınıfı
    public class UserRegistrationDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
    public class UserLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
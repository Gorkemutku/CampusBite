using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }

    // Dışarıdan sadece bu bilgileri kabul edeceğimizi belirten DTO sınıfı
    public class UserRegistrationDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
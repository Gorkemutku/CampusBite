using Exchange.API.Data;
using Exchange.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Exchange.API.Controllers
{
    [Authorize] // DİKKAT: Bu satır sayesinde buraya sadece JWT'si (geçerli kimliği) olanlar girebilir!
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ExchangeDbContext _context;

        public ItemsController(ExchangeDbContext context)
        {
            _context = context;
        }

        // GET: api/items
        // Sadece istek atan öğrencinin kendi üniversitesindeki aktif ilanları getirir
        [HttpGet]
        public async Task<IActionResult> GetItemsForMyUniversity()
        {
            // 1. JWT içinden kullanıcının okuduğu üniversitenin ID'sini çekiyoruz
            var universityIdClaim = User.FindFirst("UniversityId")?.Value;
            
            if (string.IsNullOrEmpty(universityIdClaim))
                return Unauthorized("Kimlik kartınızda üniversite bilgisi bulunamadı.");

            var universityId = Guid.Parse(universityIdClaim);

            // 2. Veritabanından sadece bu üniversiteye ait ve satılmamış ilanları getiriyoruz
            var items = await _context.Items
                .Where(i => i.UniversityId == universityId && i.IsActive && !i.IsSold)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return Ok(items);
        }

        // POST: api/items
        // Öğrencinin yeni bir takas/satış ilanı eklemesini sağlar
        // POST: api/items
[HttpPost]
public async Task<IActionResult> CreateItem(CreateItemDto request)
{
    // 1. JWT içinden bilgileri güvenli bir şekilde çekiyoruz (Alternatif isimleri de kontrol ediyoruz)
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
    var universityIdClaim = User.FindFirst("UniversityId")?.Value;

    // 2. Eğer token içinde bu bilgilerden biri bile eksikse 500 yerine anlamlı bir hata dönüyoruz
    if (string.IsNullOrEmpty(userIdClaim))
        return BadRequest("Kimlik kartınızda kullanıcı ID bilgisi (sub/NameIdentifier) bulunamadı.");

    if (string.IsNullOrEmpty(universityIdClaim))
        return BadRequest("Kimlik kartınızda üniversite ID bilgisi (UniversityId) bulunamadı.");

    // 3. String verileri güvenle Guid tipine çeviriyoruz
    var userId = Guid.Parse(userIdClaim);
    var universityId = Guid.Parse(universityIdClaim);

    // 4. Yeni eşyayı oluşturup kaydediyoruz
    var newItem = new Item
    {
        UserId = userId,
        UniversityId = universityId,
        Title = request.Title,
        Description = request.Description,
        Price = request.Price,
        ImageUrl = request.ImageUrl
    };

    _context.Items.Add(newItem);
    await _context.SaveChangesAsync();

    return Ok(newItem);
}
    }

    // Dışarıdan sadece bu bilgileri isteyeceğiz (Kullanıcı kendi ID'sini elden giremez)
    public class CreateItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}

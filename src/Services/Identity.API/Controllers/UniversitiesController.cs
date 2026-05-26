using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversitiesController : ControllerBase
    {
        private readonly IdentityDbContext _context;

        // Veritabanı köprümüzü Controller'a bağlıyoruz
        public UniversitiesController(IdentityDbContext context)
        {
            _context = context;
        }

        // GET: api/universities
        // Sisteme kayıtlı tüm üniversiteleri listeler
        [HttpGet]
        public async Task<IActionResult> GetUniversities()
        {
            var universities = await _context.Universities.ToListAsync();
            return Ok(universities);
        }

        // POST: api/universities
        // Sisteme yeni bir üniversite ekler
        [HttpPost]
        public async Task<IActionResult> AddUniversity(University university)
        {
            _context.Universities.Add(university);
            await _context.SaveChangesAsync();
            
            return Ok(university);
        }
    }
}
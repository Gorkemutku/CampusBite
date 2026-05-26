using Identity.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        // Veritabanında oluşacak tablolarımız
        public DbSet<User> Users { get; set; }
        public DbSet<University> Universities { get; set; }
    }
}
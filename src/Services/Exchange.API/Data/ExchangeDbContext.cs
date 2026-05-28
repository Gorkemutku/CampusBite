using Exchange.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Exchange.API.Data
{
    public class ExchangeDbContext : DbContext
    {
        public ExchangeDbContext(DbContextOptions<ExchangeDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fiyat alanı için SQL tarafında decimal hassasiyetini belirliyoruz
            modelBuilder.Entity<Item>()
                .Property(i => i.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
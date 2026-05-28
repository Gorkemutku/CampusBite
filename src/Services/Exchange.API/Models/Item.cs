namespace Exchange.API.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        
        // Multi-Tenant Filtresi: İlanın hangi kampüse ait olduğu
        public Guid UniversityId { get; set; }
        
        // İlanı veren öğrencinin ID'si (Identity servisinden gelecek)
        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } // 0 ise ücretsiz/takaslık denebilir
        public string ImageUrl { get; set; } = string.Empty;
        
        // İlanın durumu
        public bool IsSold { get; set; } = false;
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
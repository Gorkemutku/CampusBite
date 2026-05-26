namespace Identity.API.Models
{
    public class User
    {
        public Guid Id { get; set; }
        
        // Hangi üniversiteye ait olduğu bilgisi (Multi-Tenant kilidi)
        public Guid UniversityId { get; set; } 
        public University? University { get; set; } // Entity Framework'ün ilişkiyi anlaması için

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; } = false; // Sadece okul maili onaylananlar girebilsin
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
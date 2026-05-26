namespace Identity.API.Models
{
    public class University
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty; // Örn: "mu.edu.tr"
        public string LogoUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
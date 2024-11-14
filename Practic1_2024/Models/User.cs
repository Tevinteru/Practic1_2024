namespace Practic1_2024.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}

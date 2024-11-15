namespace Practic1_2024.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateOnly RegistrationDate { get; set; }

        // Связь с заказами
        public ICollection<Order> Orders { get; set; }
    }
}

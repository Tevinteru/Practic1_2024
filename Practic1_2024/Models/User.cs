namespace Practic1_2024.Models
{
    public class User
    {
        public int Id { get; set; }  // Уникальный идентификатор пользователя
        public string Name { get; set; }  // Имя пользователя
        public string Email { get; set; }  // Электронная почта
        public string Password { get; set; }  // Пароль
        public string Role { get; set; }  // Роль пользователя (например, "admin", "customer")
        public string Phone { get; set; }  // Телефон
        public string Address { get; set; }  // Адрес

        // Навигационное свойство: связь с заказами
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

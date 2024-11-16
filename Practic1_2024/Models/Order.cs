namespace Practic1_2024.Models
{
    public class Order
    {
        public int Id { get; set; }  // Уникальный идентификатор заказа
        public int UserId { get; set; }  // Идентификатор пользователя
        public User User { get; set; }  // Навигационное свойство для пользователя
        public decimal TotalPrice { get; set; }  // Общая стоимость заказа
        public string Status { get; set; }  // Статус заказа (например, "В обработке", "Доставлен")
        public DateTime CreatedAt { get; set; }  // Дата создания заказа
        public DateTime UpdatedAt { get; set; }  // Дата последнего обновления заказа

        // Навигационное свойство: связь с позициями в заказах
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

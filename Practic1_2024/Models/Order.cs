namespace Practic1_2024.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } // Связь с пользователем
        public DateOnly OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }

        // Связь с заказанными товарами
        public ICollection<OrderItem> OrderItems { get; set; }

        // Связь с оплатами
        public ICollection<Payment> Payments { get; set; }
    }
}

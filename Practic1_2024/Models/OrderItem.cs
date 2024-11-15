namespace Practic1_2024.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; } // Связь с заказом
        public int SmartphoneId { get; set; }
        public Smartphone? Smartphone { get; set; } // Связь с товаром
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
    }
}

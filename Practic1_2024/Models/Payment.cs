namespace Practic1_2024.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; } // Связь с заказом
        public decimal Amount { get; set; }
        public DateOnly PaymentDate { get; set; }
        public string PaymentMethod { get; set; } // Например, "Credit Card"
        public string PaymentStatus { get; set; } // Например, "Successful"
    }
}

namespace Practic1_2024.Models
{
    public class Smartphone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Manufacturer { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; } // Связь с категорией
        public int QuantityInStock { get; set; }
        public string Image { get; set; }
        public DateOnly DateAdded { get; set; }

        // Связь с заказами
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

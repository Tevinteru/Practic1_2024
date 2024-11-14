using System.ComponentModel.DataAnnotations.Schema;

namespace Practic1_2024.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? OperatingSystem { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<ProductCategory>? ProductCategories { get; set; } = new HashSet<ProductCategory>();
        [NotMapped]
        public int[] SelectedCategories { get; set; }
    }
}

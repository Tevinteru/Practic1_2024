namespace Practic1_2024.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public ICollection<ProductCategory>? ProductCategories { get; set; }
    }
}

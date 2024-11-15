namespace Practic1_2024.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Связь с товарами
        public ICollection<Smartphone> Smartphones { get; set; } = new List<Smartphone>(); // Инициализация пустой коллекцией
    }
}

namespace Practic1_2024.Models
{
    public class Brand
    {
        public int Id { get; set; }  // Уникальный идентификатор бренда
        public string Name { get; set; }  // Название бренда

        // Навигационное свойство: связь с коллекцией смартфонов
        public List<Smartphone> Smartphones { get; set; } = new List<Smartphone>();
    }
}

namespace Practic1_2024.Models
{
    public class Category
    {
        public int Id { get; set; }  // Уникальный идентификатор категории
        public string Name { get; set; }  // Название категории

        // Навигационное свойство: связь с коллекцией смартфонов
        public List<Smartphone> Smartphones { get; set; } = new List<Smartphone>();
    }
}

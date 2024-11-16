namespace Practic1_2024.Models
{
    public class Smartphone
    {
        public int Id { get; set; }  // Уникальный идентификатор смартфона
        public string Name { get; set; }  // Название смартфона
        public int BrandId { get; set; }  // Идентификатор бренда
        public Brand Brand { get; set; }  // Навигационное свойство для бренда
        public string Description { get; set; }  // Описание смартфона
        public decimal Price { get; set; }  // Цена смартфона
        public int ReleaseYear { get; set; }  // Год выпуска
        public int SimCount { get; set; }  // Количество SIM-карт
        public string MemoryOptions { get; set; }  // Опции памяти
        public string ColorOptions { get; set; }  // Опции цвета
        public int CategoryId { get; set; }  // Идентификатор категории
        public Category Category { get; set; }  // Навигационное свойство для категории
        public string ImageUrl { get; set; }  // Ссылка на изображение смартфона

        // Навигационное свойство: связь с характеристиками
        public ICollection<SmartphoneCharacteristic> Characteristics { get; set; } = new List<SmartphoneCharacteristic>();

        // Навигационное свойство: связь с позициями в заказах
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

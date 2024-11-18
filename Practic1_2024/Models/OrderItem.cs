using System.Text.Json.Serialization;

namespace Practic1_2024.Models
{
    public class OrderItem
    {
        public int Id { get; set; }  // Уникальный идентификатор позиции в заказе
        public int OrderId { get; set; }  // Идентификатор заказа
        [JsonIgnore]  // Отключаем связь с Brand для предотвращения циклов
        public Order Order { get; set; }  // Навигационное свойство для заказа
        public int SmartphoneId { get; set; }  // Идентификатор товара (связь с таблицей смартфонов)
        [JsonIgnore]  // Отключаем связь с Brand для предотвращения циклов
        public Smartphone Smartphone { get; set; }  // Навигационное свойство для товара
        public int Quantity { get; set; }  // Количество товара
        public decimal Price { get; set; }  // Цена товара на момент покупки
    }
}

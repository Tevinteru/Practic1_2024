namespace Practic1_2024.Models
{
    public class OrderItem
    {
        public int Id { get; set; }  // Уникальный идентификатор позиции в заказе
        public int OrderId { get; set; }  // Идентификатор заказа
        public Order Order { get; set; }  // Навигационное свойство для заказа
        public int ProductId { get; set; }  // Идентификатор товара (связь с таблицей смартфонов)
        public Smartphone Smartphone { get; set; }  // Навигационное свойство для товара
        public int Quantity { get; set; }  // Количество товара
        public decimal Price { get; set; }  // Цена товара на момент покупки
    }
}

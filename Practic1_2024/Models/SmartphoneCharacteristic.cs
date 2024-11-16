namespace Practic1_2024.Models
{
    public class SmartphoneCharacteristic
    {
        public int Id { get; set; }  // Уникальный идентификатор характеристики
        public int SmartphoneId { get; set; }  // Идентификатор смартфона
        public Smartphone Smartphone { get; set; }  // Навигационное свойство для смартфона
        public string Characteristic { get; set; }  // Тип характеристики (например, "Процессор")
        public string Value { get; set; }  // Значение характеристики (например, "Snapdragon 888")
    }
}

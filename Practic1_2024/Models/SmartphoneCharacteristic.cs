using System.Text.Json.Serialization;

namespace Practic1_2024.Models
{
    public class SmartphoneCharacteristic
    {
        public int Id { get; set; }  // Уникальный идентификатор характеристики
        public int SmartphoneId { get; set; }  // Идентификатор смартфона
        [JsonIgnore]  // Отключаем связь с Brand для предотвращения циклов
        public Smartphone Smartphone { get; set; }  // Навигационное свойство для смартфона
        public string Characteristic { get; set; }  // Тип характеристики (например, "Процессор")
        public string Value { get; set; }  // Значение характеристики (например, "Snapdragon 888")
    }
}

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Practic1_2024.Models
{
    public class SmartphoneCharacteristic
    {
        public int Id { get; set; }  // Уникальный идентификатор характеристики
        public int SmartphoneId { get; set; }  // Идентификатор смартфона
        [JsonIgnore]  // Отключаем связь с Brand для предотвращения циклов
        [XmlIgnore]  // Это исключает циклическую зависимость

        public Smartphone Smartphone { get; set; }  // Навигационное свойство для смартфона
        public string Characteristic { get; set; }  // Тип характеристики (например, "Процессор")
        public string Value { get; set; }  // Значение характеристики (например, "Snapdragon 888")
    }
}

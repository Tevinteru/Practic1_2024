using System.Globalization;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Practic1_2024.Data;
using Practic1_2024.Models;
using YamlDotNet.Serialization;
using System.Xml.Serialization;
using CsvHelper;
using System.Globalization;
using System.IO;

namespace Practic1_2024.Controllers
{
    public class ImportController : Controller
    {
        public void ImportFromCsv(string filePath)
        {
            var records = new List<Brand>(); // Например, импортируем бренды

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Brand>().ToList();
            }

            // Сохраняем данные в базу данных
            using (var context = new StoreDbContext(_dbContextOptions))
            {
                context.Brands.AddRange(records);
                context.SaveChanges();
            }
        }
        public void ImportFromXml(string filePath)
        {
            // Определяем тип данных, в который будем десериализовать
            var serializer = new XmlSerializer(typeof(Tables));  // Tables - класс, определяющий структуру данных
            using (var reader = new StreamReader(filePath))
            {
                var tables = (Tables)serializer.Deserialize(reader);

                // Пример вставки данных в таблицу брендов
                var brands = tables.TableList.FirstOrDefault(t => t.Name == "Brands");
                if (brands != null)
                {
                    var brandRecords = brands.Rows.Select(row => new Brand
                    {
                        Name = row.Cells[0], // Например, имя бренда в первой ячейке
                    }).ToList();

                    using (var context = new StoreDbContext(_dbContextOptions))
                    {
                        context.Brands.AddRange(brandRecords);
                        context.SaveChanges();
                    }
                }
            }
        }
        public void ImportFromTxt(string filePath)
        {
            var records = new List<Brand>();

            // Читаем файл построчно
            var lines = System.IO.File.ReadAllLines(filePath);
            if (lines.Length > 1)  // Пропускаем первую строку с заголовками
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    var cells = lines[i].Split("\t");  // Разделяем по табуляции
                    records.Add(new Brand
                    {
                        Name = cells[0], // Например, имя бренда
                    });
                }
            }

            // Сохраняем в базу данных
            using (var context = new StoreDbContext(_dbContextOptions))
            {
                context.Brands.AddRange(records);
                context.SaveChanges();
            }
        }
        public void ImportFromYaml(string filePath)
        {
            var deserializer = new Deserializer();
            var yaml = File.ReadAllText(filePath);

            // Преобразуем YAML в объекты
            var records = deserializer.Deserialize<List<Brand>>(yaml);

            // Сохраняем в базу данных
            using (var context = new StoreDbContext(_dbContextOptions))
            {
                context.Brands.AddRange(records);
                context.SaveChanges();
            }
        }
    }
}

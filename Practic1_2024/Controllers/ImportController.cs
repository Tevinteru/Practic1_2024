using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using Practic1_2024.Models;
using Microsoft.EntityFrameworkCore;
using Practic1_2024.Data;
using System.Globalization;

namespace Practic1_2024.Controllers
{
    public class ImportController : Controller
    {
        private readonly StoreDbContext _context; // Контекст базы данных
        private readonly string _importFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public ImportController(StoreDbContext context)
        {
            _context = context;

            if (!Directory.Exists(_importFolder))
            {
                Directory.CreateDirectory(_importFolder);
            }
        }

        // Действие для отображения формы загрузки файла
        public IActionResult Index()
        {
            return View();
        }

        // Действие для обработки загрузки и импорта файла
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Пожалуйста, выберите файл для загрузки.");
                return View("Index");
            }

            // Сохраняем файл на сервере
            var filePath = Path.Combine(_importFolder, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Обрабатываем файл в зависимости от его расширения
            string fileExtension = Path.GetExtension(file.FileName).ToLower();

            switch (fileExtension)
            {
                case ".txt":
                    await ImportFromTxt(filePath);
                    break;
                case ".csv":
                    await ImportFromTxt(filePath);
                    break;
                case ".xml":
                    await ImportFromXml(filePath);
                    break;
                case ".yaml":
                case ".yml":
                    await ImportFromYaml(filePath);
                    break;
                default:
                    ModelState.AddModelError("", "Неподдерживаемый формат файла.");
                    return View("Index");
            }

            // Перенаправляем пользователя после успешной загрузки
            TempData["Message"] = "Файл успешно импортирован!";
            return RedirectToAction("Index");
        }

        // Импорт данных из TXT файла
        private async Task ImportFromTxt(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            string currentTable = null;
            List<string> headers = null;

            // Словарь для соответствия таблиц и их методов импорта
            var tableImportMethods = new Dictionary<string, Func<List<string>, Task>>
            {
                { "Categories", ImportCategories },
                { "Brands", ImportBrands },
                { "Users", ImportUsers },
                { "Smartphones", ImportSmartphones },
                { "SmartphoneCharacteristics", ImportSmartphoneCharacteristics },
                { "Orders", ImportOrders },
                { "OrderItems", ImportOrderItems },
            };

            foreach (var line in lines)
            {
                // Пропускаем пустые строки
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Если строка содержит название таблицы, обновляем currentTable
                foreach (var table in tableImportMethods.Keys)
                {
                    if (line.Contains(table))
                    {
                        currentTable = table;
                        headers = null; // Очистим заголовки, так как начинаем новый блок данных
                        break;
                    }
                }

                // Если это строка с заголовками — пропускаем её
                if (headers == null)
                {
                    headers = line.Split(';').ToList(); // Сохраняем заголовки
                    continue;
                }

                // Пропускаем строку, если она пустая или с недостаточными данными
                var row = line.Split(';').ToList();
                if (row.Count == 0 || row.All(string.IsNullOrWhiteSpace)) continue;

                // Вызываем метод импорта для текущей таблицы, если она найдена
                if (currentTable != null && tableImportMethods.ContainsKey(currentTable))
                {
                    await tableImportMethods[currentTable](row);
                }
            }
        }


        //// Импорт данных из CSV файла
        //private async Task ImportFromCsv(string filePath)
        //{
        //    var lines = System.IO.File.ReadAllLines(filePath);
        //    string currentTable = null;
        //    List<string> headers = null;

        //    foreach (var line in lines)
        //    {
        //        if (string.IsNullOrWhiteSpace(line)) continue;

        //        var row = line.Split(';').ToList();

        //        if (row.Count == 1)
        //        {
        //            currentTable = row[0];
        //            continue;
        //        }

        //        // Парсим строки данных
        //        switch (currentTable)
        //        {
        //            case "Categories":
        //                await ImportCategories(row);
        //                break;
        //            case "Users":
        //                await ImportUsers(row);
        //                break;
        //            case "Brands":
        //                await ImportBrands(row);
        //                break;
        //            case "Smartphones":
        //                await ImportSmartphones(row);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        // Импорт данных из XML файла
        private async Task ImportFromXml(string filePath)
        {
            // Чтение XML в список объектов
            XmlSerializer serializer = new XmlSerializer(typeof(List<Category>)); // Здесь можно выбрать любую модель
            using (var reader = new StreamReader(filePath))
            {
                var categories = (List<Category>)serializer.Deserialize(reader);
                foreach (var category in categories)
                {
                    _context.Categories.Add(category);
                }
                await _context.SaveChangesAsync();
            }
        }

        // Импорт данных из YAML файла
        private async Task ImportFromYaml(string filePath)
        {
            // Чтение YAML
            var yamlContent = System.IO.File.ReadAllText(filePath);
            var deserializer = new Deserializer();
            var categories = deserializer.Deserialize<List<Category>>(yamlContent);

            foreach (var category in categories)
            {
                _context.Categories.Add(category);
            }

            await _context.SaveChangesAsync();
        }
        // Пример импорта данных для Categories (категорий)
        private async Task ImportCategories(List<string> columns)
        {
            try
            {
                Console.WriteLine($"Обрабатываем строку: {string.Join(";", columns)}");

                // Пропускаем строку, если это заголовок
                if (columns.Count == 1 && columns[0].ToLower() == "name")
                {
                    Console.WriteLine("Пропускаем строку с заголовками.");
                    return; // Пропускаем строку, если это заголовок
                }
                // Пропускаем строки с пустыми или неверными данными

                // Создаем новый объект категории
                var category = new Category
                {
                    Name = columns[0].Trim() // Имя категории
                };

                // Добавляем категорию в базу данных
                _context.Categories.Add(category);
                await _context.SaveChangesAsync(); // Сохраняем изменения
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте категории: {string.Join(";", columns)}. Ошибка: {ex.Message}");
            }
        }

        private async Task ImportBrands(List<string> columns)
        {
            try
            {
                Console.WriteLine($"Обрабатываем строку: {string.Join(";", columns)}");

                // Пропускаем строку, если это заголовок
                if (columns.Count == 1 && columns[0].ToLower() == "name")
                {
                    Console.WriteLine("Пропускаем строку с заголовками.");
                    return; // Пропускаем строку, если это заголовок
                }

                // Создаем новый объект бренда
                var brand = new Brand
                {
                    Name = columns[0].Trim() // Имя бренда
                };

                // Добавляем бренд в базу данных
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync(); // Сохраняем изменения
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте бренда: {string.Join(";", columns)}. Ошибка: {ex.Message}");
            }
        }


        // Пример импорта данных для Users (пользователей)
        private async Task ImportUsers(List<string> columns)
        {
            try
            {
                Console.WriteLine($"Обрабатываем строку: {string.Join(";", columns)}");

                // Пропускаем строку, если это заголовок
                if (columns.Count > 1 && columns[0].ToLower() == "name" && columns[1].ToLower() == "email")
                {
                    Console.WriteLine("Пропускаем строку с заголовками.");
                    return; // Пропускаем строку, если это заголовок
                }

                if (columns.Count < 6) return; // Пропускаем строки с недостаточными данными

                var user = new User
                {
                    Name = columns[0].Trim(),
                    Email = columns[1].Trim(),
                    Password = columns[2].Trim(),
                    Role = columns[3].Trim(),
                    Phone = columns[4].Trim(),
                    Address = columns[5].Trim()
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте пользователя: {string.Join(";", columns)}. Ошибка: {ex.Message}");
            }
        }

        // Пример импорта данных для Smartphones (смартфонов)
        private async Task ImportSmartphones(List<string> columns)
        {
            try
            {
                Console.WriteLine($"Обрабатываем строку: {string.Join(";", columns)}");

                // Пропускаем строку, если это заголовок
                if (columns.Count > 1 && columns[0].ToLower() == "name" && columns[1].ToLower() == "brandid")
                {
                    Console.WriteLine("Пропускаем строку с заголовками.");
                    return; // Пропускаем строку, если это заголовок
                }
                // Проверка на достаточность данных в строке
                if (columns.Count < 9) return; // Пропускаем строки с недостаточными данными

                // Извлекаем значения из строк
                var smartphone = new Smartphone
                {
                    Name = columns[0].Trim(), // Название смартфона
                    BrandId = Convert.ToInt32(columns[1].Trim()), // Идентификатор бренда
                    Description = columns[2].Trim(), // Описание
                    Price = Convert.ToDecimal(columns[3].Trim()), // Цена
                    ReleaseYear = Convert.ToInt32(columns[4].Trim()), // Год выпуска
                    SimCount = Convert.ToInt32(columns[5].Trim()), // Количество SIM-карт
                    MemoryOptions = columns[6].Trim(), // Опции памяти (например, "128GB, 256GB")
                    ColorOptions = columns[7].Trim(), // Опции цвета
                    CategoryId = Convert.ToInt32(columns[8].Trim()), // Идентификатор категории
                    ImageUrl = columns[9].Trim() // URL изображения
                };

                // Добавляем смартфон в базу данных
                _context.Smartphones.Add(smartphone);
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
            catch (Exception ex)
            {
                // Логирование ошибок импорта
                Console.WriteLine($"Ошибка при импорте смартфона: {string.Join(";", columns)}. Ошибка: {ex.Message}");
            }
        }

        // Пример импорта данных для SmartphoneCharacteristics (характеристик смартфонов)
        private async Task ImportSmartphoneCharacteristics(List<string> columns)
        {
            try
            {
                Console.WriteLine($"Обрабатываем строку: {string.Join(";", columns)}");
                // Пропускаем строку, если это заголовок
                if (columns.Count > 1 && columns[0].ToLower() == "smartphoneid" && columns[1].ToLower() == "characteristic")
                {
                    Console.WriteLine("Пропускаем строку с заголовками.");
                    return; // Пропускаем строку, если это заголовок
                }
                // Проверка на достаточность данных в строке
                if (columns.Count < 3) return; // Пропускаем строки с недостаточными данными

                // Извлекаем значения из строки
                var smartphoneCharacteristic = new SmartphoneCharacteristic
                {
                    SmartphoneId = Convert.ToInt32(columns[0].Trim()), // Идентификатор смартфона
                    Characteristic = columns[1].Trim(), // Характеристика (например, "Диагональ экрана")
                    Value = columns[2].Trim() // Значение характеристики (например, "6.1 дюйма")
                };

                // Добавляем характеристику смартфона в базу данных
                _context.SmartphoneCharacteristics.Add(smartphoneCharacteristic);
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
            catch (Exception ex)
            {
                // Логирование ошибок импорта
                Console.WriteLine($"Ошибка при импорте характеристики смартфона: {string.Join(";", columns)}. Ошибка: {ex.Message}");
            }
        }


        // Пример импорта данных для Orders (заказов)
        private async Task ImportOrders(List<string> columns)
        {
            try
            {
                Console.WriteLine($"Обрабатываем строку: {string.Join(";", columns)}");
                // Пропускаем строку, если это заголовок
                if (columns.Count > 1 && columns[0].ToLower() == "userid" && columns[1].ToLower() == "totalprice")
                {
                    Console.WriteLine("Пропускаем строку с заголовками.");
                    return; // Пропускаем строку, если это заголовок
                }
                if (columns.Count < 5) return; // Пропускаем строки с недостаточными данными

                var order = new Order
                {
                    UserId = Convert.ToInt32(columns[0].Trim()), // Идентификатор пользователя
                    TotalPrice = Convert.ToDecimal(columns[1].Trim()), // Общая стоимость
                    Status = columns[2].Trim(), // Статус
                    CreatedAt = DateOnly.Parse(columns[3].Trim()), // Дата создания
                    UpdatedAt = DateOnly.Parse(columns[4].Trim()) // Дата обновления
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Если есть позиции для этого заказа, добавляем их
                // Позиции будут добавлены в методе ImportOrderItems
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте заказа: {string.Join(";", columns)}. Ошибка: {ex.Message}");
            }
        }
        private async Task ImportOrderItems(List<string> columns)
        {
            try
            {
                Console.WriteLine($"Обрабатываем строку: {string.Join(";", columns)}");

                // Пропускаем строку, если это заголовок
                if (columns.Count > 1 && columns[0].ToLower() == "orderid" && columns[1].ToLower() == "smartphoneid")
                {
                    Console.WriteLine("Пропускаем строку с заголовками.");
                    return; // Пропускаем строку, если это заголовок
                }

                // Пропускаем строки с недостаточными данными
                if (columns.Count < 4) return;


                // Создание нового элемента заказа
                var orderItem = new OrderItem
                {
                    OrderId = Convert.ToInt32(columns[0].Trim()), // Идентификатор заказа
                    SmartphoneId = Convert.ToInt32(columns[1].Trim()), // Идентификатор смартфона
                    Quantity = Convert.ToInt32(columns[2].Trim()), // Количество товара
                    Price = Convert.ToDecimal(columns[3].Trim()), // Цена товара
                };
                
                // Добавление позиции в заказ
                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при импорте позиции в заказе: {string.Join(";", columns)}. Ошибка: {ex.Message}");
            }
        }



    }
}
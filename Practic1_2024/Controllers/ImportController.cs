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
using static Practic1_2024.Data.XmlClass;
using System.Xml.Linq;

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
                    await ImportXmlDataToDatabase(filePath);
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
        // Метод для обработки XML файла и импорта данных в базу данных
        private async Task ImportXmlDataToDatabase(string filePath)
        {
            // Загружаем XML документ
            XDocument xDocument = XDocument.Load(filePath);

            // Импортируем категории
            var categories = xDocument.Descendants("Categories").Elements("Row").Select(row => new Category
            {
                Name = row.Element("Name")?.Value
            }).ToList();
            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Импортируем бренды
            var brands = xDocument.Descendants("Brands").Elements("Row").Select(row => new Brand
            {
                Name = row.Element("Name")?.Value
            }).ToList();
            _context.Brands.AddRange(brands);
            await _context.SaveChangesAsync();

            // Импортируем пользователей
            var users = xDocument.Descendants("Users").Elements("Row").Select(row => new User
            {
                Name = row.Element("Name")?.Value,
                Email = row.Element("Email")?.Value,
                Password = row.Element("Password")?.Value,
                Role = row.Element("Role")?.Value,
                Phone = row.Element("Phone")?.Value,
                Address = row.Element("Address")?.Value
            }).ToList();
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            // Импортируем смартфоны
            var smartphones = xDocument.Descendants("Smartphones").Elements("Row").Select(row => new Smartphone
            {
                Name = row.Element("Name")?.Value,
                BrandId = int.Parse(row.Element("BrandId")?.Value ?? "0"),
                Description = row.Element("Description")?.Value,
                Price = decimal.Parse(row.Element("Price")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture),
                ReleaseYear = int.Parse(row.Element("ReleaseYear")?.Value ?? "0"),
                SimCount = int.Parse(row.Element("SimCount")?.Value ?? "0"),
                MemoryOptions = row.Element("MemoryOptions")?.Value,
                ColorOptions = row.Element("ColorOptions")?.Value,
                CategoryId = int.Parse(row.Element("CategoryId")?.Value ?? "0"),
                ImageUrl = row.Element("ImageUrl")?.Value
            }).ToList();
            _context.Smartphones.AddRange(smartphones);
            await _context.SaveChangesAsync();


            // Импортируем характеристики смартфонов
            var smartphoneCharacteristics = xDocument.Descendants("SmartphoneCharacteristics").Elements("Row").Select(row => new SmartphoneCharacteristic
            {
                SmartphoneId = int.Parse(row.Element("SmartphoneId")?.Value ?? "0"),
                Characteristic = row.Element("Characteristic")?.Value,
                Value = row.Element("Value")?.Value
            }).ToList();
            _context.SmartphoneCharacteristics.AddRange(smartphoneCharacteristics);
            await _context.SaveChangesAsync();

            // Импортируем заказы (Orders) - важный шаг!
            var orders = xDocument.Descendants("Orders").Elements("Row").Select(row => new Order
            {
                UserId = int.Parse(row.Element("UserId")?.Value ?? "0"),
                TotalPrice = decimal.Parse(row.Element("TotalPrice")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture),
                Status = row.Element("Status")?.Value,
                CreatedAt = DateOnly.Parse(row.Element("CreatedAt")?.Value),
                UpdatedAt = DateOnly.Parse(row.Element("UpdatedAt")?.Value)
            }).ToList();
            _context.Orders.AddRange(orders);

            // Сохраняем заказы (Orders) в базу данных, чтобы у нас были валидные OrderId для OrderItems
            await _context.SaveChangesAsync();

            // Теперь вставляем OrderItems
            var orderItems = xDocument.Descendants("OrderItems").Elements("Row").Select(row => new OrderItem
            {
                OrderId = int.Parse(row.Element("OrderId")?.Value ?? "0"), // Убедитесь, что OrderId существует в таблице Orders
                SmartphoneId = int.Parse(row.Element("SmartphoneId")?.Value ?? "0"),
                Quantity = int.Parse(row.Element("Quantity")?.Value ?? "0"),
                Price = decimal.Parse(row.Element("Price")?.Value.Replace(',', '.'), CultureInfo.InvariantCulture)
            }).ToList();
            _context.OrderItems.AddRange(orderItems);

            // Сохраняем OrderItems в базу данных
            await _context.SaveChangesAsync();
        }

        // Импорт данных из YAML файла
        private async Task ImportFromYaml(string filePath)
        {
            var yamlContent = System.IO.File.ReadAllText(filePath);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                .Build();

            // Десериализация содержимого YAML в словарь
            var data = deserializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(yamlContent);

            // Импортируем данные в соответствующие таблицы
            if (data.ContainsKey("Categories"))
            {
                foreach (var item in data["Categories"])
                {
                    var category = new Category
                    {
                        Name = item["Name"]
                    };
                    _context.Categories.Add(category);
                }
                await _context.SaveChangesAsync();
            }

            if (data.ContainsKey("Brands"))
            {
                foreach (var item in data["Brands"])
                {
                    var brand = new Brand
                    {
                        Name = item["Name"]
                    };
                    _context.Brands.Add(brand);
                }
                await _context.SaveChangesAsync();
            }

            if (data.ContainsKey("Smartphones"))
            {
                foreach (var item in data["Smartphones"])
                {
                    var smartphone = new Smartphone
                    {
                        Name = item["Name"],
                        BrandId = int.Parse(item["BrandId"]),
                        Description = item["Description"],
                        Price = decimal.Parse(item["Price"]),
                        ReleaseYear = int.Parse(item["ReleaseYear"]),
                        SimCount = int.Parse(item["SimCount"]),
                        MemoryOptions = item["MemoryOptions"],
                        ColorOptions = item["ColorOptions"],
                        CategoryId = int.Parse(item["CategoryId"]),
                        ImageUrl = item["ImageUrl"]
                    };
                    _context.Smartphones.Add(smartphone);
                }
                await _context.SaveChangesAsync();
            }

            if (data.ContainsKey("SmartphoneCharacteristics"))
            {
                foreach (var item in data["SmartphoneCharacteristics"])
                {
                    var characteristic = new SmartphoneCharacteristic
                    {
                        SmartphoneId = int.Parse(item["SmartphoneId"]),
                        Characteristic = item["Characteristic"],
                        Value = item["Value"]
                    };
                    _context.SmartphoneCharacteristics.Add(characteristic);
                }
                await _context.SaveChangesAsync();
            }

            if (data.ContainsKey("Users"))
            {
                foreach (var item in data["Users"])
                {
                    var user = new User
                    {
                        Name = item["Name"],
                        Email = item["Email"],
                        Password = item["Password"],
                        Role = item["Role"],
                        Phone = item["Phone"],
                        Address = item["Address"]
                    };
                    _context.Users.Add(user);
                }
                await _context.SaveChangesAsync();
            }

            if (data.ContainsKey("Orders"))
            {
                foreach (var item in data["Orders"])
                {
                    var order = new Order
                    {
                        UserId = int.Parse(item["UserId"]),
                        TotalPrice = decimal.Parse(item["TotalPrice"]),
                        Status = item["Status"],
                        CreatedAt = DateOnly.Parse(item["CreatedAt"]),
                        UpdatedAt = DateOnly.Parse(item["UpdatedAt"])
                    };
                    _context.Orders.Add(order);
                }
                await _context.SaveChangesAsync();
            }

            if (data.ContainsKey("OrderItems"))
            {
                foreach (var item in data["OrderItems"])
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = int.Parse(item["OrderId"]),
                        SmartphoneId = int.Parse(item["SmartphoneId"]),
                        Quantity = int.Parse(item["Quantity"]),
                        Price = decimal.Parse(item["Price"])
                    };
                    _context.OrderItems.Add(orderItem);
                }
                await _context.SaveChangesAsync();
            }

            // Сохраняем все изменения в базе данных
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
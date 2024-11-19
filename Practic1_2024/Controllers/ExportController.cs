using Microsoft.AspNetCore.Mvc;
using Practic1_2024.Data;
using Practic1_2024.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

namespace Practic1_2024.Controllers
{
    public class ExportController : Controller
    {
        private readonly StoreDbContext _context;

        public ExportController(StoreDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        // Метод для экспорта данных в CSV
        public IActionResult ExportToCsv()
        {
            var categories = _context.Categories.ToList();
            var brands = _context.Brands.ToList();
            var users = _context.Users.ToList();
            var smartphones = _context.Smartphones.ToList();
            var characteristic = _context.SmartphoneCharacteristics.ToList();
            var orders = _context.Orders.ToList();
            var orderItems = _context.OrderItems.ToList();

            var sb = new StringBuilder();

            // Экспорт категорий
            sb.AppendLine("Categories");
            sb.AppendLine("Name");
            for (int i = 0; i < categories.Count; i++)
            {
                sb.AppendLine(categories[i].Name);
            }

            // Экспорт брендов
            sb.AppendLine("\nBrands");
            sb.AppendLine("Name");
            for (int i = 0; i < brands.Count; i++)
            {
                sb.AppendLine(brands[i].Name);
            }

            // Экспорт пользователей
            sb.AppendLine("\nUsers");
            sb.AppendLine("Name;Email;Password;Role;Phone;Address");
            for (int i = 0; i < users.Count; i++)
            {
                sb.AppendLine($"{users[i].Name};{users[i].Email};{users[i].Password};{users[i].Role};{users[i].Phone};{users[i].Address}");
            }

            // Экспорт смартфонов
            sb.AppendLine("\nSmartphones");
            sb.AppendLine("Name;BrandId;Description;Price;ReleaseYear;SimCount;MemoryOptions;ColorOptions;CategoryId;ImageUrl");
            for (int i = 0; i < smartphones.Count; i++)
            {
                sb.AppendLine($"{smartphones[i].Name};{smartphones[i].BrandId};{smartphones[i].Description};{smartphones[i].Price};{smartphones[i].ReleaseYear};{smartphones[i].SimCount};{smartphones[i].MemoryOptions};{smartphones[i].ColorOptions};{smartphones[i].CategoryId};{smartphones[i].ImageUrl}");
            }

            // Экспорт характеристик смартфонов
            sb.AppendLine("\nSmartphoneCharacteristics");
            sb.AppendLine("SmartphoneId;Characteristic;Value");
            for (int i = 0; i < characteristic.Count; i++)
            {
                sb.AppendLine($"{characteristic[i].SmartphoneId};{characteristic[i].Characteristic};{characteristic[i].Value}");
            }
            // Экспорт заказов
            sb.AppendLine("\nOrders");
            sb.AppendLine("UserId;TotalPrice;Status;CreatedAt;UpdatedAt");
            for (int i = 0; i < orders.Count; i++)
            {
                sb.AppendLine($"{orders[i].UserId};{orders[i].TotalPrice};{orders[i].Status};{orders[i].CreatedAt};{orders[i].UpdatedAt}");
            }

            // Экспорт позиций в заказах
            sb.AppendLine("\nOrderItems");
            sb.AppendLine("OrderId;SmartphoneId;Quantity;Price");
            for (int i = 0; i < orderItems.Count; i++)
            {
                sb.AppendLine($"{orderItems[i].OrderId};{orderItems[i].SmartphoneId};{orderItems[i].Quantity};{orderItems[i].Price}");
            }

            var fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "text/csv", "export.csv");
        }


        // Метод для экспорта данных в TXT
        public IActionResult ExportToTxt()
        {
            var categories = _context.Categories.ToList();
            var brands = _context.Brands.ToList();
            var users = _context.Users.ToList();
            var smartphones = _context.Smartphones.ToList();
            var characteristic = _context.SmartphoneCharacteristics.ToList();
            var orders = _context.Orders.ToList();
            var orderItems = _context.OrderItems.ToList();

            var sb = new StringBuilder();

            // Экспорт категорий
            sb.AppendLine("Categories");
            sb.AppendLine("Name");
            for (int i = 0; i < categories.Count; i++)
            {
                sb.AppendLine(categories[i].Name);
            }

            // Экспорт брендов
            sb.AppendLine("\nBrands");
            sb.AppendLine("Name");
            for (int i = 0; i < brands.Count; i++)
            {
                sb.AppendLine(brands[i].Name);
            }

            // Экспорт пользователей
            sb.AppendLine("\nUsers");
            sb.AppendLine("Name;Email;Password;Role;Phone;Address");
            for (int i = 0; i < users.Count; i++)
            {
                sb.AppendLine($"{users[i].Name};{users[i].Email};{users[i].Password};{users[i].Role};{users[i].Phone};{users[i].Address}");
            }

            // Экспорт смартфонов
            sb.AppendLine("\nSmartphones");
            sb.AppendLine("Name;BrandId;Description;Price;ReleaseYear;SimCount;MemoryOptions;ColorOptions;CategoryId;ImageUrl");
            for (int i = 0; i < smartphones.Count; i++)
            {
                sb.AppendLine($"{smartphones[i].Name};{smartphones[i].BrandId};{smartphones[i].Description};{smartphones[i].Price};{smartphones[i].ReleaseYear};{smartphones[i].SimCount};{smartphones[i].MemoryOptions};{smartphones[i].ColorOptions};{smartphones[i].CategoryId};{smartphones[i].ImageUrl}");
            }

            // Экспорт характеристик смартфонов
            sb.AppendLine("\nSmartphoneCharacteristics");
            sb.AppendLine("SmartphoneId;Characteristic;Value");
            for (int i = 0; i < characteristic.Count; i++)
            {
                sb.AppendLine($"{characteristic[i].SmartphoneId};{characteristic[i].Characteristic};{characteristic[i].Value}");
            }

            // Экспорт заказов
            sb.AppendLine("\nOrders");
            sb.AppendLine("UserId;TotalPrice;Status;CreatedAt;UpdatedAt");
            for (int i = 0; i < orders.Count; i++)
            {
                sb.AppendLine($"{orders[i].UserId};{orders[i].TotalPrice};{orders[i].Status};{orders[i].CreatedAt};{orders[i].UpdatedAt}");
            }

            // Экспорт позиций в заказах
            sb.AppendLine("\nOrderItems");
            sb.AppendLine("OrderId;SmartphoneId;Quantity;Price");
            for (int i = 0; i < orderItems.Count; i++)
            {
                sb.AppendLine($"{orderItems[i].OrderId};{orderItems[i].SmartphoneId};{orderItems[i].Quantity};{orderItems[i].Price}");
            }

            var fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "text/plain", "export.txt");
        }


        public IActionResult ExportToXml()
        {
            // Получаем данные из базы данных
            var categories = _context.Categories.ToList();
            var brands = _context.Brands.ToList();
            var users = _context.Users.ToList();
            var smartphones = _context.Smartphones.ToList();
            var smartphoneCharacteristics = _context.SmartphoneCharacteristics.ToList();
            var orders = _context.Orders.ToList();
            var orderItems = _context.OrderItems.ToList();

            var sb = new StringBuilder();

            // Начинаем формировать XML
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<Tables>");

            // Экспорт категорий
            sb.AppendLine("<Categories>");
            foreach (var category in categories)
            {
                sb.AppendLine($"  <Row><Name>{category.Name}</Name></Row>");
            }
            sb.AppendLine("</Categories>");

            // Экспорт брендов
            sb.AppendLine("<Brands>");
            foreach (var brand in brands)
            {
                sb.AppendLine($"  <Row><Name>{brand.Name}</Name></Row>");
            }
            sb.AppendLine("</Brands>");

            // Экспорт пользователей
            sb.AppendLine("<Users>");
            foreach (var user in users)
            {
                sb.AppendLine($"  <Row><Name>{user.Name}</Name><Email>{user.Email}</Email><Password>{user.Password}</Password><Role>{user.Role}</Role><Phone>{user.Phone}</Phone><Address>{user.Address}</Address></Row>");
            }
            sb.AppendLine("</Users>");

            // Экспорт смартфонов
            sb.AppendLine("<Smartphones>");
            foreach (var smartphone in smartphones)
            {
                sb.AppendLine($"  <Row><Name>{smartphone.Name}</Name><BrandId>{smartphone.BrandId}</BrandId><Description>{smartphone.Description}</Description><Price>{smartphone.Price.ToString("F2", CultureInfo.InvariantCulture)}</Price><ReleaseYear>{smartphone.ReleaseYear}</ReleaseYear><SimCount>{smartphone.SimCount}</SimCount><MemoryOptions>{smartphone.MemoryOptions}</MemoryOptions><ColorOptions>{smartphone.ColorOptions}</ColorOptions><CategoryId>{smartphone.CategoryId}</CategoryId><ImageUrl>{smartphone.ImageUrl}</ImageUrl></Row>");
            }
            sb.AppendLine("</Smartphones>");

            // Экспорт характеристик смартфонов
            sb.AppendLine("<SmartphoneCharacteristics>");
            foreach (var characteristic in smartphoneCharacteristics)
            {
                sb.AppendLine($"  <Row><SmartphoneId>{characteristic.SmartphoneId}</SmartphoneId><Characteristic>{characteristic.Characteristic}</Characteristic><Value>{characteristic.Value}</Value></Row>");
            }
            sb.AppendLine("</SmartphoneCharacteristics>");

            // Экспорт заказов
            sb.AppendLine("<Orders>");
            foreach (var order in orders)
            {
                sb.AppendLine($"  <Row><UserId>{order.UserId}</UserId><TotalPrice>{order.TotalPrice.ToString("F2", CultureInfo.InvariantCulture)}</TotalPrice><Status>{order.Status}</Status><CreatedAt>{order.CreatedAt:dd.MM.yyyy}</CreatedAt><UpdatedAt>{order.UpdatedAt:dd.MM.yyyy}</UpdatedAt></Row>");
            }
            sb.AppendLine("</Orders>");

            // Экспорт позиций в заказах
            sb.AppendLine("<OrderItems>");
            foreach (var orderItem in orderItems)
            {
                sb.AppendLine($"  <Row><OrderId>{orderItem.OrderId}</OrderId><SmartphoneId>{orderItem.SmartphoneId}</SmartphoneId><Quantity>{orderItem.Quantity}</Quantity><Price>{orderItem.Price.ToString("F2", CultureInfo.InvariantCulture)}</Price></Row>");
            }
            sb.AppendLine("</OrderItems>");

            sb.AppendLine("</Tables>");

            // Преобразуем строку в массив байтов и возвращаем файл
            var fileBytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(fileBytes, "application/xml", "export.xml");
        }

        public IActionResult ExportToYaml()
        {
            // Получаем данные из базы данных
            var categories = _context.Categories.ToList();
            var brands = _context.Brands.ToList();
            var users = _context.Users.ToList();
            var smartphones = _context.Smartphones.ToList();
            var smartphoneCharacteristics = _context.SmartphoneCharacteristics.ToList();
            var orders = _context.Orders.ToList();
            var orderItems = _context.OrderItems.ToList();

            // Создаем словарь для YAML
            var data = new Dictionary<string, List<Dictionary<string, string>>>
        {
            { "Categories", categories.Select(c => new Dictionary<string, string> { { "Name", c.Name } }).ToList() },
            { "Brands", brands.Select(b => new Dictionary<string, string> { { "Name", b.Name } }).ToList() },
            { "Users", users.Select(u => new Dictionary<string, string>
                {
                    { "Name", u.Name },
                    { "Email", u.Email },
                    { "Password", u.Password },
                    { "Role", u.Role },
                    { "Phone", u.Phone },
                    { "Address", u.Address }
                }).ToList() },
            { "Smartphones", smartphones.Select(s => new Dictionary<string, string>
                {
                    { "Name", s.Name },
                    { "BrandId", s.BrandId.ToString() },
                    { "Description", s.Description },
                    { "Price", s.Price.ToString("F2", CultureInfo.InvariantCulture) },
                    { "ReleaseYear", s.ReleaseYear.ToString() },
                    { "SimCount", s.SimCount.ToString() },
                    { "MemoryOptions", s.MemoryOptions },
                    { "ColorOptions", s.ColorOptions },
                    { "CategoryId", s.CategoryId.ToString() },
                    { "ImageUrl", s.ImageUrl }
                }).ToList() },
            { "SmartphoneCharacteristics", smartphoneCharacteristics.Select(sc => new Dictionary<string, string>
                {
                    { "SmartphoneId", sc.SmartphoneId.ToString() },
                    { "Characteristic", sc.Characteristic },
                    { "Value", sc.Value }
                }).ToList() },
            { "Orders", orders.Select(o => new Dictionary<string, string>
                {
                    { "UserId", o.UserId.ToString() },
                    { "TotalPrice", o.TotalPrice.ToString("F2", CultureInfo.InvariantCulture) },
                    { "Status", o.Status },
                    { "CreatedAt", o.CreatedAt.ToString("dd.MM.yyyy") },
                    { "UpdatedAt", o.UpdatedAt.ToString("dd.MM.yyyy") }
                }).ToList() },
            { "OrderItems", orderItems.Select(oi => new Dictionary<string, string>
                {
                    { "OrderId", oi.OrderId.ToString() },
                    { "SmartphoneId", oi.SmartphoneId.ToString() },
                    { "Quantity", oi.Quantity.ToString() },
                    { "Price", oi.Price.ToString("F2", CultureInfo.InvariantCulture) }
                }).ToList() }
        };

            // Используем YamlDotNet для сериализации данных в YAML формат
            var serializer = new SerializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                .Build();

            var yamlContent = serializer.Serialize(data);

            // Возвращаем YAML в ответе как файл
            var fileBytes = Encoding.UTF8.GetBytes(yamlContent);
            return File(fileBytes, "application/x-yaml", "export.yaml");
        }
    }
}

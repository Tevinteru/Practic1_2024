using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using YamlDotNet.Serialization;

namespace Practic1_2024.Controllers
{
    public class ConverterController : Controller
    {
        // Папка для сохранения экспортированных файлов
        private readonly string _downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads");

        public ConverterController()
        {
            // Создаем каталог, если его нет
            Directory.CreateDirectory(_downloadFolder);
        }

        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Upload(IFormFile excelFile)
        {
            if (excelFile?.Length > 0)
            {
                var filePath = Path.Combine(_downloadFolder, excelFile.FileName);
                SaveFile(excelFile, filePath);

                var downloadLinks = ProcessFileAndExport(filePath);

                ViewData["DownloadLinks"] = downloadLinks;
                return View("Index");
            }

            ModelState.AddModelError("", "Пожалуйста, выберите файл для загрузки.");
            return View("Index");
        }

        // Сохраняем файл на сервере
        private void SaveFile(IFormFile file, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
        }

        // Обрабатываем Excel файл и экспортируем в различные форматы
        private List<string> ProcessFileAndExport(string filePath)
        {
            var downloadLinks = new List<string>();

            var allData = ReadExcelData(filePath);

            // Экспортируем в различные форматы
            downloadLinks.Add(ExportToTxt(allData));
            downloadLinks.Add(ExportToCsv(allData));
            downloadLinks.Add(ExportToXml(allData));
            downloadLinks.Add(ExportToYaml(allData));

            return downloadLinks;
        }

        // Чтение данных из Excel
        private Dictionary<string, List<Dictionary<string, string>>> ReadExcelData(string filePath)
        {
            var allData = new Dictionary<string, List<Dictionary<string, string>>>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    var sheetData = ReadWorksheet(worksheet);
                    allData[worksheet.Name] = sheetData;
                }
            }

            return allData;
        }

        // Чтение данных с листа Excel
        private List<Dictionary<string, string>> ReadWorksheet(ExcelWorksheet worksheet)
        {
            var data = new List<Dictionary<string, string>>();
            var headers = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns]
                           .Select(cell => cell.Text)
                           .ToList();

            for (int row = 2; row <= worksheet.Dimension.Rows; row++)
            {
                var rowData = new Dictionary<string, string>();
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    rowData[headers[col - 1]] = worksheet.Cells[row, col].Text;
                }
                data.Add(rowData);
            }

            return data;
        }

        // Экспорт в формат .txt
        private string ExportToTxt(Dictionary<string, List<Dictionary<string, string>>> allData)
        {
            var filePath = Path.Combine(_downloadFolder, "all_tables.txt");
            var sb = new StringBuilder();

            foreach (var table in allData)
            {
                sb.AppendLine(table.Key);
                sb.AppendLine(string.Join(";", table.Value.First().Keys));
                foreach (var row in table.Value)
                {
                    sb.AppendLine(string.Join(";", row.Values));
                }
                sb.AppendLine(); // Пустая строка между таблицами
            }

            System.IO.File.WriteAllText(filePath, sb.ToString().TrimEnd());
            return $"/downloads/{Path.GetFileName(filePath)}";
        }

        // Экспорт в формат .csv
        private string ExportToCsv(Dictionary<string, List<Dictionary<string, string>>> allData)
        {
            var filePath = Path.Combine(_downloadFolder, "all_tables.csv");
            var sb = new StringBuilder();

            foreach (var table in allData)
            {
                sb.AppendLine(table.Key);
                sb.AppendLine(string.Join(";", table.Value.First().Keys));
                foreach (var row in table.Value)
                {
                    sb.AppendLine(string.Join(";", row.Values));
                }
                sb.AppendLine(); // Пустая строка между таблицами
            }

            System.IO.File.WriteAllText(filePath, sb.ToString().TrimEnd());
            return $"/downloads/{Path.GetFileName(filePath)}";
        }

        // Экспорт в формат .xml
        private string ExportToXml(Dictionary<string, List<Dictionary<string, string>>> allData)
        {
            var filePath = Path.Combine(_downloadFolder, "all_tables.xml");
            var xDocument = new XDocument(
                new XElement("Tables",
                    allData.Select(table =>
                        new XElement(SanitizeXmlName(table.Key),
                            table.Value.Select(row =>
                                new XElement("Row",
                                    row.Select(col =>
                                        new XElement(SanitizeXmlName(col.Key), col.Value))))))));

            xDocument.Save(filePath);
            return $"/downloads/{Path.GetFileName(filePath)}";
        }

        // Экспорт в формат .yaml
        private string ExportToYaml(Dictionary<string, List<Dictionary<string, string>>> allData)
        {
            var filePath = Path.Combine(_downloadFolder, "all_tables.yaml");
            var serializer = new SerializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                .Build();

            var yamlContent = serializer.Serialize(allData);
            System.IO.File.WriteAllText(filePath, yamlContent.TrimEnd());

            return $"/downloads/{Path.GetFileName(filePath)}";
        }

        // Санитизация имен для XML
        private string SanitizeXmlName(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? "Unnamed" : new string(name.Select(c =>
                char.IsLetterOrDigit(c) || c == '_' ? c : '_').ToArray());
        }
    }
}

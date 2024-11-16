using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using System.Runtime.Serialization;

namespace Practic1_2024.Controllers
{
    public class ConverterController : Controller
    {
        // Статический каталог для хранения экспортируемых файлов
        private readonly string _downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads");

        public ConverterController()
        {
            // Создаем каталог, если его нет
            if (!Directory.Exists(_downloadFolder))
            {
                Directory.CreateDirectory(_downloadFolder);
            }
        }

        // Действие для отображения формы загрузки
        public IActionResult Index()
        {
            return View();
        }

        // Действие для обработки загрузки Excel файла
        [HttpPost]
        public IActionResult UploadFile(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Пожалуйста, выберите файл для загрузки.");
                return View("Index");
            }

            // Сохраняем загруженный файл на сервере
            var filePath = Path.Combine(_downloadFolder, excelFile.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                excelFile.CopyTo(fileStream);
            }

            // Чтение данных из Excel файла и экспорт в другие форматы
            var downloadLinks = ProcessFileAndExport(filePath, _downloadFolder);

            // Передаем ссылки на скачивание в представление
            ViewData["DownloadLinks"] = downloadLinks;

            return View("Index");
        }

        // Метод для обработки Excel файла и экспорта данных в несколько форматов
        public List<string> ProcessFileAndExport(string filePath, string downloadFolder)
        {
            var downloadLinks = new List<string>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var allData = new Dictionary<string, List<Dictionary<string, string>>>();

                // Перебор всех листов в Excel файле
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    var sheetData = ReadWorksheet(worksheet);
                    allData[worksheet.Name] = sheetData;
                }

                // Экспортируем все таблицы в один файл каждого формата
                // .txt
                var txtFilePath = Path.Combine(downloadFolder, "all_tables.txt");
                ExportToTxt(allData, txtFilePath);
                downloadLinks.Add($"/downloads/{Path.GetFileName(txtFilePath)}");

                // .csv
                var csvFilePath = Path.Combine(downloadFolder, "all_tables.csv");
                ExportToCsv(allData, csvFilePath);
                downloadLinks.Add($"/downloads/{Path.GetFileName(csvFilePath)}");

                // .xml
                var xmlFilePath = Path.Combine(downloadFolder, "all_tables.xml");
                ExportToXml(allData, xmlFilePath);
                downloadLinks.Add($"/downloads/{Path.GetFileName(xmlFilePath)}");

                // .yaml
                var yamlFilePath = Path.Combine(downloadFolder, "all_tables.yaml");
                ExportToYaml(allData, yamlFilePath);
                downloadLinks.Add($"/downloads/{Path.GetFileName(yamlFilePath)}");
            }

            return downloadLinks;
        }

        // Метод для чтения данных с листа Excel
        private List<Dictionary<string, string>> ReadWorksheet(ExcelWorksheet worksheet)
        {
            var data = new List<Dictionary<string, string>>();
            var rowCount = worksheet.Dimension.Rows;
            var columnCount = worksheet.Dimension.Columns;

            // Прочитать заголовки
            var headers = new List<string>();
            for (int col = 1; col <= columnCount; col++)
            {
                headers.Add(worksheet.Cells[1, col].Text);
            }

            // Прочитать данные и создать словари для каждой строки
            for (int row = 2; row <= rowCount; row++)
            {
                var rowData = new Dictionary<string, string>();
                for (int col = 1; col <= columnCount; col++)
                {
                    var header = headers[col - 1];
                    var value = worksheet.Cells[row, col].Text;
                    rowData[header] = value;
                }
                data.Add(rowData);
            }

            return data;
        }

        // Метод для экспорта данных во все форматы (для одного файла)
        private void ExportToTxt(Dictionary<string, List<Dictionary<string, string>>> allData, string filePath)
        {
            var sb = new StringBuilder();

            // Для каждой таблицы добавляем название таблицы и ее содержимое
            foreach (var table in allData)
            {
                sb.AppendLine($"### {table.Key} ###");

                // Записываем заголовки
                if (table.Value.Count > 0)
                {
                    var headers = table.Value.First().Keys.ToList();
                    sb.AppendLine(string.Join("\t", headers));
                }

                // Записываем строки
                foreach (var row in table.Value)
                {
                    sb.AppendLine(string.Join("\t", row.Values));
                }

                sb.AppendLine();
            }

            // Сохраняем файл
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        private void ExportToCsv(Dictionary<string, List<Dictionary<string, string>>> allData, string filePath)
        {
            var sb = new StringBuilder();

            // Для каждой таблицы добавляем название таблицы и ее содержимое
            foreach (var table in allData)
            {
                sb.AppendLine($"### {table.Key} ###");

                // Записываем заголовки
                if (table.Value.Count > 0)
                {
                    var headers = table.Value.First().Keys.ToList();
                    sb.AppendLine(string.Join(",", headers));
                }

                // Записываем строки
                foreach (var row in table.Value)
                {
                    sb.AppendLine(string.Join(",", row.Values));
                }

                sb.AppendLine();
            }

            // Сохраняем файл
            System.IO.File.WriteAllText(filePath, sb.ToString());
        }

        private void ExportToXml(Dictionary<string, List<Dictionary<string, string>>> allData, string filePath)
        {
            var serializer = new DataContractSerializer(typeof(Dictionary<string, List<Dictionary<string, string>>>));

            using (var writer = new FileStream(filePath, FileMode.Create))
            {
                serializer.WriteObject(writer, allData);
            }
        }

        private void ExportToYaml(Dictionary<string, List<Dictionary<string, string>>> allData, string filePath)
        {
            var serializer = new Serializer();
            var yamlContent = serializer.Serialize(allData);
            System.IO.File.WriteAllText(filePath, yamlContent);
        }
    }
}

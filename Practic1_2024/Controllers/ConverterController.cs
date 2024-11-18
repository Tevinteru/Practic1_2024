﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using System.Runtime.Serialization;
using System.Xml.Linq;
using static Practic1_2024.Data.XmlClass;

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
        public IActionResult Upload(IFormFile excelFile)
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

        private void ExportToTxt(Dictionary<string, List<Dictionary<string, string>>> allData, string filePath)
        {
            var sb = new StringBuilder();

            // Для каждой таблицы добавляем название таблицы и ее содержимое
            foreach (var table in allData)
            {
                sb.AppendLine($"{table.Key}");

                // Записываем заголовки
                if (table.Value.Count > 0)
                {
                    var headers = table.Value.First().Keys.ToList();
                    sb.AppendLine(string.Join(";", headers));
                }

                // Записываем строки
                foreach (var row in table.Value)
                {
                    // Проверяем, что строка не пустая, чтобы избежать добавления пустых строк
                    var rowData = string.Join(";", row.Values);
                    if (!string.IsNullOrWhiteSpace(rowData))
                    {
                        sb.AppendLine(rowData);
                    }
                }

                // Убираем добавление лишней пустой строки после каждой таблицы
                sb.AppendLine(); // Оставляем эту строку, если нужно, но можно убрать, если она лишняя
            }

            // Сохраняем файл
            System.IO.File.WriteAllText(filePath, sb.ToString().TrimEnd()); // .TrimEnd() удаляет последние пустые строки
        }
        private void ExportToCsv(Dictionary<string, List<Dictionary<string, string>>> allData, string filePath)
        {
            var sb = new StringBuilder();

            // Для каждой таблицы добавляем название таблицы и ее содержимое
            foreach (var table in allData)
            {
                sb.AppendLine($"{table.Key}");

                // Записываем заголовки
                if (table.Value.Count > 0)
                {
                    var headers = table.Value.First().Keys.ToList();
                    sb.AppendLine(string.Join(";", headers));
                }

                // Записываем строки
                foreach (var row in table.Value)
                {
                    // Проверяем, что строка не пустая, чтобы избежать добавления пустых строк
                    var rowData = string.Join(";", row.Values);
                    if (!string.IsNullOrWhiteSpace(rowData))
                    {
                        sb.AppendLine(rowData);
                    }
                }

                // Убираем добавление лишней пустой строки после каждой таблицы
                sb.AppendLine(); // Оставляем эту строку, если нужно, но можно убрать, если она лишняя
            }

            // Сохраняем файл
            System.IO.File.WriteAllText(filePath, sb.ToString().TrimEnd()); // .TrimEnd() удаляет последние пустые строки
        }

        private void ExportToXml(Dictionary<string, List<Dictionary<string, string>>> allData, string filePath)
        {
            var xDocument = new XDocument();
            var rootElement = new XElement("Tables"); // Корневой элемент для всех таблиц

            // Перебираем все таблицы в данных
            foreach (var table in allData)
            {
                var tableElement = new XElement(SanitizeXmlName(table.Key)); // Элемент для каждой таблицы
                var headers = table.Value.First().Keys.ToList(); // Заголовки

                // Перебираем строки данных для таблицы
                foreach (var row in table.Value)
                {
                    var rowElement = new XElement("Row"); // Элемент строки
                    foreach (var header in headers)
                    {
                        // Преобразуем заголовок в допустимое имя XML элемента
                        var sanitizedHeader = SanitizeXmlName(header);
                        var columnElement = new XElement(sanitizedHeader, row[header]); // Элемент для каждой ячейки
                        rowElement.Add(columnElement);
                    }
                    tableElement.Add(rowElement); // Добавляем строку в таблицу
                }
                rootElement.Add(tableElement); // Добавляем таблицу в корневой элемент
            }

            xDocument.Add(rootElement); // Добавляем все таблицы в XML

            // Сохраняем XML файл
            xDocument.Save(filePath);
        }

        // Метод для удаления или замены недопустимых символов в имени XML элемента
        private string SanitizeXmlName(string name)
        {
            // Заменяем пробелы на подчеркивание
            return string.IsNullOrWhiteSpace(name) ? "Unnamed" : new string(name.Select(c =>
            {
                // Убираем все символы, которые не могут быть использованы в именах XML элементов
                return char.IsLetterOrDigit(c) || c == '_' ? c : '_';
            }).ToArray());
        }


        public void ExportToYaml(Dictionary<string, List<Dictionary<string, string>>> allData, string yamlFilePath)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                .Build();

            // Преобразуем данные в формат YAML
            var yamlContent = serializer.Serialize(allData);

            // Убираем лишние пробелы в конце контента
            yamlContent = yamlContent.TrimEnd();

            // Сохраняем данные в файл
            System.IO.File.WriteAllText(yamlFilePath, yamlContent);
        }

    }
}

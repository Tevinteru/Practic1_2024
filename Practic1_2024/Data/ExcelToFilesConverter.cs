using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using YamlDotNet.Serialization;

public class ExcelToFilesConverter
{
    public static void ConvertExcelToFiles(string inputFilePath, string outputDirectory)
    {
        // Открываем Excel-файл
        FileInfo fileInfo = new FileInfo(inputFilePath);
        using (var package = new ExcelPackage(fileInfo))
        {
            // Перебираем все листы Excel
            foreach (var worksheet in package.Workbook.Worksheets)
            {
                var tableName = worksheet.Name;
                var rows = GetTableRows(worksheet);

                // Создаем файлы
                CreateCsvFile(rows, tableName, outputDirectory);
                CreateTxtFile(rows, tableName, outputDirectory);
                CreateXmlFile(rows, tableName, outputDirectory);
                CreateYamlFile(rows, tableName, outputDirectory);
            }
        }
    }

    private static List<List<string>> GetTableRows(ExcelWorksheet worksheet)
    {
        var rows = new List<List<string>>();

        int rowCount = worksheet.Dimension.Rows;
        int colCount = worksheet.Dimension.Columns;

        // Читаем строки и столбцы
        for (int row = 1; row <= rowCount; row++)
        {
            var rowData = new List<string>();
            for (int col = 1; col <= colCount; col++)
            {
                rowData.Add(worksheet.Cells[row, col].Text);
            }
            rows.Add(rowData);
        }

        return rows;
    }

    private static void CreateCsvFile(List<List<string>> rows, string tableName, string outputDirectory)
    {
        var sb = new StringBuilder();

        // Записываем заголовки
        sb.AppendLine(string.Join(";", rows[0]));

        // Записываем данные
        for (int i = 1; i < rows.Count; i++)
        {
            sb.AppendLine(string.Join(";", rows[i]));
        }

        // Сохраняем в файл
        File.WriteAllText(Path.Combine(outputDirectory, $"{tableName}.csv"), sb.ToString(), Encoding.UTF8);
    }

    private static void CreateTxtFile(List<List<string>> rows, string tableName, string outputDirectory)
    {
        var sb = new StringBuilder();

        // Записываем заголовки
        sb.AppendLine(string.Join(";", rows[0]));

        // Записываем данные
        for (int i = 1; i < rows.Count; i++)
        {
            sb.AppendLine(string.Join(";", rows[i]));
        }

        // Сохраняем в файл
        File.WriteAllText(Path.Combine(outputDirectory, $"{tableName}.txt"), sb.ToString());
    }

    private static void CreateXmlFile(List<List<string>> rows, string tableName, string outputDirectory)
    {
        var xDoc = new XDocument();
        var root = new XElement(tableName);

        // Добавляем строки в XML
        for (int i = 1; i < rows.Count; i++)
        {
            var rowElement = new XElement("Row");
            for (int j = 0; j < rows[0].Count; j++)
            {
                rowElement.Add(new XElement(rows[0][j], rows[i][j]));
            }
            root.Add(rowElement);
        }

        xDoc.Add(root);

        // Сохраняем в файл
        xDoc.Save(Path.Combine(outputDirectory, $"{tableName}.xml"));
    }

    private static void CreateYamlFile(List<List<string>> rows, string tableName, string outputDirectory)
    {
        var yamlData = new List<Dictionary<string, string>>();

        // Перебираем строки и создаем объекты
        for (int i = 1; i < rows.Count; i++)
        {
            var rowDict = new Dictionary<string, string>();
            for (int j = 0; j < rows[0].Count; j++)
            {
                rowDict[rows[0][j]] = rows[i][j];
            }
            yamlData.Add(rowDict);
        }

        // Сериализуем в YAML
        var serializer = new Serializer();
        var yamlContent = serializer.Serialize(yamlData);

        // Сохраняем в файл
        File.WriteAllText(Path.Combine(outputDirectory, $"{tableName}.yaml"), yamlContent);
    }
}


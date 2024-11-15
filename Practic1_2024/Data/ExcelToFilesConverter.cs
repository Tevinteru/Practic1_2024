using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

public static class ExcelToFilesConverter
{
    public static void ConvertExcelToFiles(string inputFilePath, string outputDirectory)
    {
        // Путь к файлам
        var csvFilePath = Path.Combine(outputDirectory, "output.csv");
        var txtFilePath = Path.Combine(outputDirectory, "output.txt");
        var xmlFilePath = Path.Combine(outputDirectory, "output.xml");
        var yamlFilePath = Path.Combine(outputDirectory, "output.yaml");

        // Чтение данных из Excel
        var allData = ReadExcelFile(inputFilePath);

        // Запись данных в разные форматы
        WriteToCsv(csvFilePath, allData);
        WriteToTxt(txtFilePath, allData);
        WriteToXml(xmlFilePath, allData);
        WriteToYaml(yamlFilePath, allData);
    }

    private static Dictionary<string, List<Dictionary<string, object>>> ReadExcelFile(string filePath)
    {
        var result = new Dictionary<string, List<Dictionary<string, object>>>();

        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            foreach (var worksheet in package.Workbook.Worksheets)
            {
                var tableName = worksheet.Name;
                var rows = new List<Dictionary<string, object>>();

                // Получаем все данные из текущего листа
                var columns = worksheet.Dimension.Columns;
                var columnNames = new List<string>();

                // Чтение заголовков колонок (первая строка)
                for (int col = 1; col <= columns; col++)
                {
                    columnNames.Add(worksheet.Cells[1, col].Text.Trim());
                }

                // Чтение данных (с 2-й строки)
                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var rowData = new Dictionary<string, object>();

                    for (int col = 1; col <= columns; col++)
                    {
                        var columnName = columnNames[col - 1];
                        var cellValue = worksheet.Cells[row, col].Text?.Trim(); // Убираем пробелы
                        rowData[columnName] = string.IsNullOrEmpty(cellValue) ? null : cellValue;
                    }

                    rows.Add(rowData);
                }

                result[tableName] = rows;
            }
        }

        return result;
    }

    private static void WriteToCsv(string filePath, Dictionary<string, List<Dictionary<string, object>>> allData)
    {
        var sb = new StringBuilder();

        foreach (var table in allData)
        {
            sb.AppendLine($"Table: {table.Key}");

            var firstRow = table.Value[0];
            sb.AppendLine(string.Join(";", firstRow.Keys)); // Записываем заголовки

            foreach (var row in table.Value)
            {
                sb.AppendLine(string.Join(";", row.Values)); // Записываем значения
            }

            sb.AppendLine(); // Пустая строка между таблицами
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }


    private static void WriteToTxt(string filePath, Dictionary<string, List<Dictionary<string, object>>> allData)
    {
        var sb = new StringBuilder();

        foreach (var table in allData)
        {
            sb.AppendLine($"Table: {table.Key}");

            var firstRow = table.Value[0];
            sb.AppendLine(string.Join(";", firstRow.Keys)); // Записываем заголовки

            foreach (var row in table.Value)
            {
                sb.AppendLine(string.Join(";", row.Values)); // Записываем значения
            }

            sb.AppendLine(); // Пустая строка между таблицами
        }

        File.WriteAllText(filePath, sb.ToString());
    }


    private static void WriteToXml(string filePath, Dictionary<string, List<Dictionary<string, object>>> allData)
    {
        var wrapperList = new Dictionary<string, List<Dictionary<string, object>>>();

        foreach (var table in allData)
        {
            // Сериализация только данных для БД
            wrapperList[table.Key] = table.Value;
        }

        var xmlSerializer = new XmlSerializer(typeof(Dictionary<string, List<Dictionary<string, object>>>));
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            xmlSerializer.Serialize(stream, wrapperList);
        }
    }


    private static void WriteToYaml(string filePath, Dictionary<string, List<Dictionary<string, object>>> allData)
    {
        var wrapperList = new Dictionary<string, List<Dictionary<string, object>>>();

        foreach (var table in allData)
        {
            // Сериализация только данных, которые должны быть вставлены в БД
            wrapperList[table.Key] = table.Value;
        }

        var serializer = new Serializer();
        var yaml = serializer.Serialize(wrapperList);

        // Запись результата в файл
        File.WriteAllText(filePath, yaml);
    }

}

public class TableWrapper
{
    [XmlAttribute("TableName")]
    public string TableName { get; set; }

    [XmlElement("Row")]
    public List<TableRow> Rows { get; set; }
}

public class TableRow
{
    [XmlElement("Column")]
    public List<ColumnData> Columns { get; set; }
}

public class ColumnData
{
    [XmlAttribute("Name")]
    public string Name { get; set; }

    [XmlText]
    public string Value { get; set; }
}

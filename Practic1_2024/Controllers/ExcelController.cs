using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace Practic1_2024.Controllers
{
    public class ExcelController : Controller
    {
        private readonly string _inputFilePath = "input.xlsx";  // Путь к вашему Excel файлу
        private readonly string _outputDirectory = "output";    // Путь к директории для сохранения файлов


        [HttpPost]
        public IActionResult ExportExcel()
        {
            try
            {
                // Создаем директорию, если она не существует
                Directory.CreateDirectory(_outputDirectory);

                // Вызовите код из класса ExcelToTextConverter для конвертации Excel в файлы
                ExcelToFilesConverter.ConvertExcelToFiles(_inputFilePath, _outputDirectory);

                // Можно отобразить сообщение пользователю о том, что операция завершена
                ViewData["Message"] = "Экспорт данных завершен!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
           

            return Redirect("/");
        }
    }
}

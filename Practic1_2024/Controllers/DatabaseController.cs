using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Practic1_2024.Data;
using System.Threading.Tasks;

namespace Practic1_2024.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly StoreDbContext _context;

        public DatabaseController(StoreDbContext context)
        {
            _context = context;
        }

        // Страница с кнопкой для очистки базы данных
        public IActionResult Index()
        {
            return View();
        }

        // Метод для очистки всех таблиц
        [HttpPost]
        public async Task<IActionResult> TruncateTables()
        {
            try
            {
                // Получаем строку подключения из конфигурации
                var connectionString = _context.Database.GetDbConnection().ConnectionString;

                // Выполнение TRUNCATE для каждой таблицы через Npgsql
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Начинаем транзакцию для выполнения нескольких команд
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // Список таблиц, которые нужно очистить
                            var tables = new[]
                            {
                                "Categories",
                                "Brands",
                                "Smartphones",
                                "SmartphoneCharacteristics",
                                "Users",
                                "Orders",
                                "OrderItems"
                            };

                            foreach (var table in tables)
                            {
                                var truncateCommand = $"TRUNCATE TABLE \"{table}\" RESTART IDENTITY CASCADE;";
                                using (var command = new NpgsqlCommand(truncateCommand, connection, transaction))
                                {
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            // Завершаем транзакцию
                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            // В случае ошибки откатываем транзакцию
                            await transaction.RollbackAsync();
                            TempData["Error"] = $"Ошибка при очистке таблиц: {ex.Message}";
                            return RedirectToAction("Index");
                        }
                    }
                }

                TempData["Message"] = "Все таблицы успешно очищены.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ошибка при подключении к базе данных: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}

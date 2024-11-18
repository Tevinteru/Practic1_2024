using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Practic1_2024.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Добавление сервиса DbContext с использованием строки подключения
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging() // Логирование данных запроса
           .LogTo(Console.WriteLine, LogLevel.Information); // Логирование всех SQL запросов в консоль
});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

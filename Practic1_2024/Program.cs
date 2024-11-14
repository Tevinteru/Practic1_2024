using Microsoft.EntityFrameworkCore;
using Practic1_2024.Data;

var builder = WebApplication.CreateBuilder(args);

// Получение строки подключения из конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Добавление сервиса DbContext с использованием строки подключения
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseNpgsql(connectionString));

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

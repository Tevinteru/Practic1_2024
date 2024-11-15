using Practic1_2024.Models;

namespace Practic1_2024.Data
{
    public class DbSeeder
    {
        public static void SeedCategories(StoreDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(new Category
                {
                    Name = "Флагманы",
                    Description = "Смартфоны с передовыми технологиями и высокой производительностью"
                },
                new Category
                {
                    Name = "Средний класс",
                    Description = "Смартфоны с хорошим балансом цена/качество"
                },
                new Category
                {
                    Name = "Бюджетные",
                    Description = "Доступные смартфоны для базовых нужд"
                },
                new Category
                {
                    Name = "Игровые",
                    Description = "Смартфоны, ориентированные на геймеров с высокими характеристиками"
                });

                context.SaveChanges();
            }
        }
        public static void SeedSmartphones(StoreDbContext context)
        {
            if (!context.Smartphones.Any())
            {
                var flagshipCategory = context.Categories.First(c => c.Name == "Флагманы");
                var midrangeCategory = context.Categories.First(c => c.Name == "Средний класс");
                var budgetCategory = context.Categories.First(c => c.Name == "Бюджетные");
                var gamingCategory = context.Categories.First(c => c.Name == "Игровые");

                context.Smartphones.AddRange(new Smartphone
                {
                    Name = "iPhone 15 Pro",
                    Description = "Флагманский смартфон от Apple с 120 Гц дисплеем и чипом A17",
                    Price = 1099.99m,
                    Manufacturer = "Apple",
                    Category = flagshipCategory,
                    QuantityInStock = 50,
                    Image = "iphone15pro.jpg",
                    DateAdded = DateOnly.FromDateTime(DateTime.Now)
                },
                new Smartphone
                {
                    Name = "Samsung Galaxy S23",
                    Description = "Лучший флагман от Samsung с камерами высокого разрешения",
                    Price = 999.99m,
                    Manufacturer = "Samsung",
                    Category = flagshipCategory,
                    QuantityInStock = 30,
                    Image = "galaxys23.jpg",
                    DateAdded = DateOnly.FromDateTime(DateTime.Now)
                },
                new Smartphone
                {
                    Name = "Xiaomi Redmi Note 12",
                    Description = "Доступный смартфон с хорошими характеристиками",
                    Price = 249.99m,
                    Manufacturer = "Xiaomi",
                    Category = budgetCategory,
                    QuantityInStock = 100,
                    Image = "redminote12.jpg",
                    DateAdded = DateOnly.FromDateTime(DateTime.Now)
                },
                new Smartphone
                {
                    Name = "Asus ROG Phone 6",
                    Description = "Игровой смартфон с мощным процессором и экраном 165 Гц",
                    Price = 899.99m,
                    Manufacturer = "Asus",
                    Category = gamingCategory,
                    QuantityInStock = 20,
                    Image = "rogphone6.jpg",
                    DateAdded = DateOnly.FromDateTime(DateTime.Now)
                });

                context.SaveChanges();
            }
        }

    }
}

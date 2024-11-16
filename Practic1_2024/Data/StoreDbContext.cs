using Microsoft.EntityFrameworkCore;
using Practic1_2024.Models;

namespace Practic1_2024.Data
{
    public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
    {
        // Таблицы в базе данных
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Smartphone> Smartphones { get; set; }
        public DbSet<SmartphoneCharacteristic> SmartphoneCharacteristics { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройки для связей между таблицами
            modelBuilder.Entity<Smartphone>()
                .HasOne(s => s.Brand)
                .WithMany(b => b.Smartphones)
                .HasForeignKey(s => s.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Smartphone>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Smartphones)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SmartphoneCharacteristic>()
                .HasOne(sc => sc.Smartphone)
                .WithMany(s => s.Characteristics)
                .HasForeignKey(sc => sc.SmartphoneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Smartphone)
                .WithMany(s => s.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    

  

   

   

    

    
}

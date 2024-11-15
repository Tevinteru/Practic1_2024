using Microsoft.EntityFrameworkCore;
using Practic1_2024.Models;

namespace Practic1_2024.Data
{
    public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Smartphone> Smartphones { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь между Order и User (1 ко многим)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Связь между Order и Payment (1 ко многим)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);

            // Связь между Order и OrderItem (1 ко многим)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            // Связь между OrderItem и Smartphone (многие ко многим)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Smartphone)
                .WithMany(s => s.OrderItems)
                .HasForeignKey(oi => oi.SmartphoneId);
        }
    }
    

  

   

   

    

    
}

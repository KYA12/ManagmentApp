using BakeryOrderManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;

public class BakeryDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrdersProducts> OrdersProducts { get; set; }

    public BakeryDbContext(DbContextOptions<BakeryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrdersProducts>()
            .HasKey(op => op.OrderProductId);

        modelBuilder.Entity<OrdersProducts>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrdersProducts)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrdersProducts>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrdersProducts)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "Chocolate Cake", Description = "Delicious chocolate cake", Price = 15.99m, IsActive = true },
            new Product { ProductId = 2, Name = "Croissant", Description = "Buttery croissant", Price = 2.99m, IsActive = false },
            new Product { ProductId = 3, Name = "Apple Pie", Description = "Sweet apple pie", Price = 12.99m, IsActive = true }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order { OrderId = 1, CustomerId = 1, OrderDate = DateTime.Now, Status = OrderStatus.Pending },
            new Order { OrderId = 2, CustomerId = 2, OrderDate = DateTime.Now, Status = OrderStatus.Completed }
        );

        modelBuilder.Entity<OrdersProducts>().HasData(
            new OrdersProducts { OrderProductId = 1, OrderId = 1, ProductId = 1, Quantity = 2 },
            new OrdersProducts { OrderProductId = 2, OrderId = 1, ProductId = 2, Quantity = 3 },
            new OrdersProducts { OrderProductId = 3, OrderId = 2, ProductId = 3, Quantity = 1 }
        );

        base.OnModelCreating(modelBuilder);
    }
}
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace WebBanQuanAo.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<InventoryHistory> InventoryHistories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<FlashSale> FlashSales { get; set; }
        public DbSet<CategoryPromotion> CategoryPromotions { get; set; }
        public DbSet<ColorProduct> ColorProducts { get; set; }
        public DbSet<SizeProduct> SizeProducts { get; set; }
        public DbSet<ImageProduct> ImageProducts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

       

            // Cấu hình cho bảng Orders và OrderDetails
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId);

            // Cấu hình precision cho các trường decimal
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // Cấu hình cho ColorProduct
            modelBuilder.Entity<ColorProduct>()
                .HasOne(cp => cp.Product)
                .WithMany(p => p.Colors)
                .HasForeignKey(cp => cp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình cho SizeProduct
            modelBuilder.Entity<SizeProduct>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.Sizes)
                .HasForeignKey(sp => sp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình cho ImageProduct
            modelBuilder.Entity<ImageProduct>()
                .HasOne(ip => ip.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(ip => ip.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}

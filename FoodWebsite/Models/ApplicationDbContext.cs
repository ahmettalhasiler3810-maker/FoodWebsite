using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodWebsite.Models;

namespace FoodWebsite.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product config (CreatedDate ekledim, default tarih)
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(200);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");  // Otomatik tarih
                entity.HasIndex(e => e.Name);  // Arama için index
            });

            // CartItem config (UserId ve ProductId FK)
            builder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Quantity).IsRequired().HasDefaultValue(1);
                entity.HasIndex(e => e.UserId);  // Kullanıcı sepeti için index
            });

            // Order config (UserId FK)
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.UserId);  // Kullanıcı siparişleri için index
            });

            // OrderItem config (OrderId ve ProductId FK)
            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);  // Ürün silinirken sipariş kalır
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Quantity).IsRequired();
                entity.HasIndex(e => e.OrderId);  // Sipariş item'ları için index
            });

            // Seed data (test için, ilk run'da ekle – CreatedDate'li)
            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Pizza Margherita", Description = "Classic Italian pizza.", Price = 15.99m, Category = "Pizza", ImageUrl = "https://prnt.sc/f5i93pCk-2dA", CreatedDate = DateTime.UtcNow },
                new Product { Id = 2, Name = "Burg Burger", Description = "Juicy beef burger.", Price = 12.50m, Category = "Burger", ImageUrl = "https://prnt.sc/mGzIVU0_gdLI", CreatedDate = DateTime.UtcNow }
            );
        }
    }
}
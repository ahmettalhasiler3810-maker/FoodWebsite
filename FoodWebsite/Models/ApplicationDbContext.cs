using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodWebsite.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
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
        // SADECE BUNU BIRAK, GERİSİNİ SİK
        base.OnModelCreating(builder);

        // Product seed data - sadece bunu bırakabilirsin
        builder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Pizza Margherita",
                Description = "Classic Italian pizza.",
                Price = 15.99m,
                Category = "Pizza",
                ImageUrl = "https://prnt.sc/f5i93pCk-2dA",
                CreatedDate = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Burg Burger",
                Description = "Juicy beef burger.",
                Price = 12.50m,
                Category = "Burger",
                ImageUrl = "https://prnt.sc/mGzIVU0_gdLI",
                CreatedDate = DateTime.UtcNow
            }
        );
    }

}
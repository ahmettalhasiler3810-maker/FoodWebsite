using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FoodWebsite.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity - EN BAS�T �EK�LDE
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // �ifre kurallar�n� basitle�tir
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// MVC ve Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Admin kullan�c�s� olu�tur
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Database'in haz�r olmas�n� bekle
        await context.Database.MigrateAsync();

        // Admin kullan�c�s�n� olu�tur
        var adminUser = await userManager.FindByEmailAsync("admin@food.com");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@food.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "123");
            if (result.Succeeded)
            {
                Console.WriteLine("? ADMIN KULLANICI OLU�TURULDU!");
                Console.WriteLine("?? Email: admin@food.com");
                Console.WriteLine("?? �ifre: 123");
            }
            else
            {
                Console.WriteLine("? ADMIN OLU�TURULAMADI:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($" - {error.Description}");
                }
            }
        }
        else
        {
            Console.WriteLine("? ADMIN ZATEN VAR: admin@food.com / 123");
        }

        // Test kullan�c�s�
        var testUser = await userManager.FindByEmailAsync("test@test.com");
        if (testUser == null)
        {
            testUser = new IdentityUser
            {
                UserName = "test",
                Email = "test@test.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(testUser, "123");
            Console.WriteLine("? TEST USER: test@test.com / 123");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database haz�rlama hatas�!");
    }
}

app.Run();
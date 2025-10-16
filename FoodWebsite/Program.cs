using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity - EN BASÝT HALÝ
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

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

// ?? AREA ROUTE (Admin, User vs. gibi area'lar için)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// ?? NORMAL ROUTE
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// KULLANICI OLUÞTUR
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Veritabanýný temizle ve oluþtur
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        Console.WriteLine("? VERÝTABANI OLUÞTURULDU!");

        // ADMIN KULLANICISI
        var admin = new IdentityUser
        {
            UserName = "admin",
            Email = "admin@food.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "123");
        if (result.Succeeded)
            Console.WriteLine("? ADMIN: admin / 123");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"?? HATA: {ex.Message}");
    }
}

app.Run();

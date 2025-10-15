using Microsoft.AspNetCore.Mvc;
using FoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Linq;

namespace FoodWebsite.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            var cartItems = new List<CartItem>();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    cartItems = await _context.CartItems
                        .Where(c => c.UserId == userId)
                        .Include(c => c.Product)
                        .ToListAsync();
                }
            }

            ViewBag.CartItems = cartItems;
            return View(products);
        }

        
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

      
        [AllowAnonymous]
        public async Task<IActionResult> Menu()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        
        [AllowAnonymous]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        
        [AllowAnonymous]
        public IActionResult Review()
        {
            return View();
        }

        // Contact
        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }

        // Blogs
        [AllowAnonymous]
        public IActionResult Blogs()
        {
            return View();
        }

        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, message = "Arama terimi boþ!" });
            }

            var results = await _context.Products
                .Where(p => p.Name.Contains(query) ||
                            p.Description.Contains(query) ||
                            p.Category.Contains(query))
                .Take(10)
                .ToListAsync();

            return Json(new { success = true, count = results.Count, products = results });
        }

        // Admin Bypass - Test amaçlý admin giriþi
        [AllowAnonymous]
        public IActionResult AdminBypass()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Email, "admin@food.com"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties).Wait();

            TempData["Success"] = "Admin olarak giriþ yaptýn! (Bypass modu)";
            return RedirectToAction("Products", "Admin");
        }
    }
}

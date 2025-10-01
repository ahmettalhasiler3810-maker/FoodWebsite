using Microsoft.AspNetCore.Mvc;
using FoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FoodWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            if (User.Identity.IsAuthenticated)
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == User.Identity.Name)
                    .Include(c => c.Product)
                    .ToListAsync();
                ViewBag.CartItems = cartItems;
            }
            else
            {
                ViewBag.CartItems = new List<CartItem>();
            }
            return View(products);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }

        public IActionResult Review()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Blogs()
        {
            return View();
        }
    }
}
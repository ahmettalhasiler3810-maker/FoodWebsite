using Microsoft.AspNetCore.Mvc;
using FoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;

namespace FoodWebsite.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        protected void SetCartItems()
        {
            List<CartItem> cartItems = new List<CartItem>();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    cartItems = _context.CartItems
                        .Where(c => c.UserId == userId)
                        .Include(c => c.Product)
                        .ToList();
                }
            }
            ViewBag.CartItems = cartItems;
        }
    }
}
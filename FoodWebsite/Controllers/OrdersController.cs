using Microsoft.AspNetCore.Mvc;
using FoodWebsite.Models;
using FoodWebsite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FoodWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, EmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("Confirm")]
        [Authorize]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Unauthorized();

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == model.UserId)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Sepet boş.");

            var order = new Order
            {
                UserId = model.UserId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cartItems.Sum(c => c.Quantity * c.Product.Price),
                Status = "Confirmed",
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Product.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            await _emailService.SendOrderConfirmationEmail(user.Email, order);
            return Ok();
        }
    }

    public class ConfirmOrderModel
    {
        public string UserId { get; set; }
    }
}
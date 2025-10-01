using Microsoft.AspNetCore.Mvc;
using FoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FoodWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.Identity.Name;
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ToListAsync();
            return Ok(cartItems);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            item.UserId = User.Identity.Name;
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == item.UserId && c.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                _context.CartItems.Add(item);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null || item.UserId != User.Identity.Name)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
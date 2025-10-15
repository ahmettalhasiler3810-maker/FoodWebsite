using Microsoft.AspNetCore.Mvc;
using FoodWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodWebsite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FastCartController : ControllerBase  // İSİM DEĞİŞTİRDİM
    {
        private readonly ApplicationDbContext _context;

        public FastCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemModel model)
        {
            try
            {
                Console.WriteLine("FASTCART ÇALIŞIYOR! ProductId: " + model.ProductId);

                // GEÇİCİ USER ID - HERKES EKLEYEBİLSİN
                var tempUserId = "temp-user-" + Guid.NewGuid().ToString();

                var cartItem = new CartItem
                {
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    UserId = tempUserId
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Ürün sepete eklendi!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("HATA: " + ex.Message);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CartItemModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
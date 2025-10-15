using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FoodWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Index - Tek sayfa (ViewBag.Products ve ViewBag.Users doldur)
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            ViewBag.Products = products ?? new List<Product>();

            var users = await _userManager.Users.ToListAsync();
            var rolesDict = new Dictionary<string, string>();
            foreach (var user in users ?? new List<ApplicationUser>())
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesDict[user.Id] = string.Join(", ", roles);
            }
            ViewBag.Users = users ?? new List<ApplicationUser>();
            ViewBag.Roles = rolesDict;

            return View();
        }

        // GetProduct (JS fetch için, edit form doldur)
        [HttpGet]
        public async Task<JsonResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { error = "Product not found" });
            }
            return Json(new { id = product.Id, name = product.Name, description = product.Description, price = product.Price, category = product.Category, imageUrl = product.ImageUrl });
        }

        // Create POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Category,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün eklendi!";
            }
            else
            {
                TempData["Error"] = "Ürün eklenemedi, hataları kontrol et.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Edit POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Category,ImageUrl")] Product product)
        {
            if (id != product.Id)
            {
                TempData["Error"] = "Ürün bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Ürün güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "Güncelleme hatası.";
                }
            }
            else
            {
                TempData["Error"] = "Güncelleme hatası, formu kontrol et.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Delete POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün silindi!";
            }
            else
            {
                TempData["Error"] = "Ürün bulunamadı.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var rolesDict = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                rolesDict[user.Id] = string.Join(", ", roles);
            }
            ViewBag.Roles = rolesDict;
            return View(users);
        }
    }
}
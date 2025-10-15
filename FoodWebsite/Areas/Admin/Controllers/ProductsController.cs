using Microsoft.AspNetCore.Mvc;
using FoodWebsite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FoodWebsite.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products/Index - Liste
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            ViewBag.Products = products; // ViewBag'e products ekleyin

            // USERLARI EKLE - Basit çözüm
            try
            {
                // Eğer ApplicationDbContext'inizde Users tablosu varsa
                var users = await _context.Users.ToListAsync();
                ViewBag.Users = users;

                // Rolleri oluştur
                var userRoles = new Dictionary<string, string>();
                foreach (var user in users)
                {
                    // Basit rol ataması - gerçek uygulamada Identity kullanın
                    userRoles[user.Id] = "User"; // Varsayılan rol
                }
                ViewBag.Roles = userRoles;
            }
            catch (Exception ex)
            {
                // Eğer hata olursa boş liste göster
                ViewBag.Users = new List<ApplicationUser>();
                ViewBag.Roles = new Dictionary<string, string>();

                // Hata mesajını loglayabilirsiniz
                System.Diagnostics.Debug.WriteLine($"User loading error: {ex.Message}");
            }

            return View(products);
        }

        public IActionResult Error()
        {
            return View();
        }

        // GET: Products/Details/5 - Detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Error();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return Error();
            }

            return View(product);  // Tek Product model
        }

        // GET: Products/Create - Ekle form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create - Ekle kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Category,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.UtcNow;  // Tarih set et
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün eklendi!";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5 - Düzenle form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5 - Düzenle kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Category,ImageUrl,CreatedDate")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
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
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5 - Sil confirm
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Error();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return Error();
            }

            return View(product);
        }

        // POST: Products/Delete/5 - Sil kaydet
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün silindi!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
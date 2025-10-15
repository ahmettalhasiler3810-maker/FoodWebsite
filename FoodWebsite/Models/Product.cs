using System;
using System.ComponentModel.DataAnnotations;

namespace FoodWebsite.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad zorunlu!")]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Fiyat zorunlu!")]
        [Range(0.01, 999.99, ErrorMessage = "Fiyat 0.01 - 999.99 arası olsun!")]
        public decimal Price { get; set; }
        [StringLength(50)]
        public string Category { get; set; }
        [StringLength(200)]
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;  // Varsayılan tarih
    }
}
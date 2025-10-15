using System.ComponentModel.DataAnnotations;

namespace FoodWebsite.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalı.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrarı")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor!")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string UserName { get; set; }
    }
}
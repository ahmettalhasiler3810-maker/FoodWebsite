using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FoodWebsite.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // REGISTER
        [HttpGet] public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Kayıt başarılı! Giriş yapın.";
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // LOGIN - KESİN ÇÖZÜM
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // 1. YÖNTEM: Direkt username ile login dene
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Giriş başarılı!";
                    return RedirectToAction("Index", "Home");
                }

                // 2. YÖNTEM: Email ile kullanıcı bul, username ile login dene
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Giriş başarılı!";
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Geçersiz giriş!");
            }
            return View(model);
        }

        // LOGOUT
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Çıkış yapıldı!";
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }
    }

    // VIEWMODELS
    public class LoginViewModel
    {
        [Required][EmailAddress] public string Email { get; set; }
        [Required][DataType(DataType.Password)] public string Password { get; set; }
        [Display(Name = "Beni hatırla")] public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required][EmailAddress][Display(Name = "E-posta")] public string Email { get; set; }
        [Required][StringLength(100, MinimumLength = 1)][DataType(DataType.Password)] public string Password { get; set; }
        [DataType(DataType.Password)][Compare("Password")] public string ConfirmPassword { get; set; }
    }
}
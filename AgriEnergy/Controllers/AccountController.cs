using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AgriEnergy.Models;
using AgriEnergy.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace AgriEnergy.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Employee"))
                        return RedirectToAction("EmployeeDashboard", "Employee");
                    if (roles.Contains("Farmer"))
                        return RedirectToAction("FarmerDashboard", "Farmer");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
























/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AgriEnergy.Models;
using System.Threading.Tasks;
using AgriEnergy.ViewModels;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Employee")) return RedirectToAction("EmployeeDashboard", "Employee");
                if (roles.Contains("Farmer")) return RedirectToAction("FarmerDashboard", "Farmer");
            }
        }

        ModelState.AddModelError("", "Invalid login");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}*/

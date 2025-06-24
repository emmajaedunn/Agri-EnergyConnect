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
        // Injected services for managing users and sign-in operations
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constructor initializes SignInManager and UserManager via dependency injection
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: /Account/Login
        // Returns the login view for the user
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        // Handles user login logic
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Validate the login form input
            if (!ModelState.IsValid)
                return View(model);

            // Attempt to find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                // Try to sign in the user with the provided password
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    // Retrieve roles assigned to the user
                    var roles = await _userManager.GetRolesAsync(user);
                    
                    // Redirect user based on their role
                    if (roles.Contains("Employee"))
                        return RedirectToAction("EmployeeDashboard", "Employee");
                    
                    if (roles.Contains("Farmer"))
                        return RedirectToAction("FarmerDashboard", "Farmer");
                }
            }
            // If login fails, show a generic error
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // POST: /Account/Logout
        // Signs the user out of the application
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}



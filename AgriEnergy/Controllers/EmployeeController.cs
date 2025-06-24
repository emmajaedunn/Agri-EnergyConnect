using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriEnergy.Data;
using AgriEnergy.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AgriEnergy.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult EmployeeDashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddFarmer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(Farmer farmer)
        {
            if (!ModelState.IsValid)
            {
                // Log or display model validation errors
                return View(farmer);
            }

            // Check if user/email already exists
            var existingUser = await _userManager.FindByEmailAsync(farmer.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                return View(farmer);
            }

            var user = new ApplicationUser
            {
                UserName = farmer.Email,
                Email = farmer.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "Farm123!");
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    Console.WriteLine($"User creation error: {error.Description}");
                }
                return View(farmer);
            }

            await _userManager.AddToRoleAsync(user, "Farmer");

            farmer.UserId = user.Id;

            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();

            TempData["FarmerRegistered"] = $"Farmer registered successfully! Login: {farmer.Email} | Password: Farm123!";
            return RedirectToAction(nameof(FarmerRegistrationSuccess));
        }

        public IActionResult FarmerRegistrationSuccess()
        {
            return View();
        }

        public IActionResult ViewFarmers(string categoryFilter = "", DateTime? productionDate = null)
        {
            bool isFiltering = !string.IsNullOrEmpty(categoryFilter) || productionDate.HasValue;

            var query = _context.Farmers.Include(f => f.Products).AsQueryable();

            if (isFiltering)
            {
                // Filter farmers who have matching products
                query = query.Where(f => f.Products.Any(p =>
                    (string.IsNullOrEmpty(categoryFilter) || p.ProductCategory.Contains(categoryFilter)) &&
                    (!productionDate.HasValue || p.ProductionDate.Date == productionDate.Value.Date)
                ));
            }

            var farmers = query.ToList();

            // Filter products inside each farmer for the filter case
            if (isFiltering)
            {
                foreach (var farmer in farmers)
                {
                    farmer.Products = farmer.Products
                        .Where(p =>
                            (string.IsNullOrEmpty(categoryFilter) || p.ProductCategory.Contains(categoryFilter, StringComparison.OrdinalIgnoreCase))
                            && (!productionDate.HasValue || p.ProductionDate.Date == productionDate.Value.Date)
                        )
                        .ToList();
                }
            }

            ViewBag.IsFiltering = isFiltering;

            return View(farmers);
        }





        /*public IActionResult ViewFarmers(string categoryFilter = "", DateTime? productionDate = null)
        {
            var farmers = _context.Farmers
                .Include(f => f.Products)
                .ToList();

            foreach (var farmer in farmers)
            {
                if (!string.IsNullOrEmpty(categoryFilter))
                {
                    farmer.Products = farmer.Products
                        .Where(p => p.ProductCategory.Contains(categoryFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (productionDate.HasValue)
                {
                    var date = productionDate.Value.Date;
                    farmer.Products = farmer.Products
                        .Where(p => p.ProductionDate.Date == date)
                        .ToList();
                }
            }

            // NO filtering of farmers here, so all farmers show up

            return View(farmers);
        }*/


        public IActionResult ManageFarmers()
        {
            var farmers = _context.Farmers.ToList();
            return View(farmers);
        }

        [HttpGet]
        public IActionResult EditFarmer(int id)
        {
            var farmer = _context.Farmers.Find(id);
            if (farmer == null) return NotFound();

            return View(farmer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditFarmer(Farmer farmer)
        {
            if (ModelState.IsValid)
            {
                _context.Farmers.Update(farmer);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Farmer updated successfully!";
                return RedirectToAction("ManageFarmers");
            }
            return View(farmer);
        }

        [HttpGet]
        public IActionResult DeleteFarmer(int id)
        {
            var farmer = _context.Farmers.Find(id);
            if (farmer != null)
            {
                _context.Farmers.Remove(farmer);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Farmer deleted successfully!";
            }
            return RedirectToAction("ManageFarmers");
        }
    }
}













/*
[Authorize(Roles = "Employee")]
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult EmployeeDashboard() => View();

    [HttpGet]
    public IActionResult AddFarmer() => View();

    [HttpPost]
    public IActionResult AddFarmer(Farmer farmer)
    {
        if (ModelState.IsValid)
        {
            _context.Farmers.Add(farmer);
            _context.SaveChanges();

            TempData["FarmerRegistered"] = "Farmer registered successfully!";
            return RedirectToAction("FarmerRegistrationSuccess");
        }

        return View(farmer);
    }


    public IActionResult FarmerRegistrationSuccess()
    {
        return View();
    }

    public IActionResult ViewFarmers(string categoryFilter = "", DateTime? productionDate = null)
    {
        var farmers = _context.Farmers
            .Include(f => f.Products)
            .AsQueryable();

        // Apply filters only if needed
        if (!string.IsNullOrEmpty(categoryFilter))
        {
            farmers = farmers.Where(f => f.Products.Any(p => EF.Functions.Like(p.ProductCategory, $"%{categoryFilter}%")));
        }

        if (productionDate.HasValue)
        {
            var date = productionDate.Value.Date;
            farmers = farmers.Where(f => f.Products.Any(p => p.ProductionDate.Date == date));
        }

        return View(farmers.ToList()); // This should return all farmers, not filtered by login
    }


    public IActionResult ManageFarmers()
    {
        var farmers = _context.Farmers.ToList();
        return View(farmers);
    }

    [HttpGet]
    public IActionResult EditFarmer(int id)
    {
        var farmer = _context.Farmers.Find(id);
        if (farmer == null) return NotFound();

        return View(farmer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditFarmer(Farmer farmer)
    {
        if (ModelState.IsValid)
        {
            _context.Farmers.Update(farmer);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Farmer updated successfully!";
            return RedirectToAction("ManageFarmers");
        }
        return View(farmer);
    }

    [HttpGet]
    public IActionResult DeleteFarmer(int id)
    {
        var farmer = _context.Farmers.Find(id);
        if (farmer != null)
        {
            _context.Farmers.Remove(farmer);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Farmer deleted successfully!";
        }
        return RedirectToAction("ManageFarmers");
    }
}
*/
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
    // Only users with the "Employee" role can access this controller
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor to inject required services
        public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Employee dashboard landing page
        public IActionResult EmployeeDashboard()
        {
            return View();
        }

        // GET: Show the form to add a new farmer
        [HttpGet]
        public IActionResult AddFarmer()
        {
            return View();
        }

        // POST: Handles creation of new farmer user and farmer profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFarmer(Farmer farmer)
        {
            // Validate the input form
            if (!ModelState.IsValid)
            {
                return View(farmer);
            }

            // Check if a user with this email already exists
            var existingUser = await _userManager.FindByEmailAsync(farmer.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                return View(farmer);
            }

            // Create a new ApplicationUser for login
            var user = new ApplicationUser
            {
                UserName = farmer.Email,
                Email = farmer.Email,
                EmailConfirmed = true
            };

            // Create the user with a default password
            var result = await _userManager.CreateAsync(user, "Farm123!");
            if (!result.Succeeded)
            {
                // Show errors if user creation fails
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    Console.WriteLine($"User creation error: {error.Description}");
                }
                return View(farmer);
            }

            // Assign the new user to the "Farmer" role
            await _userManager.AddToRoleAsync(user, "Farmer");

            // Link the ApplicationUser to the Farmer profile
            farmer.UserId = user.Id;

            // Save farmer profile to the database
            _context.Farmers.Add(farmer);
            await _context.SaveChangesAsync();

            // TempData used to pass success message to success page
            TempData["FarmerRegistered"] = $"Farmer registered successfully! Login: {farmer.Email} | Password: Farm123!";
            return RedirectToAction(nameof(FarmerRegistrationSuccess));
        }

        // Displays a success message after registering a farmer
        public IActionResult FarmerRegistrationSuccess()
        {
            return View();
        }

        // Displays a list of farmers and optionally filters by product category or production date
        public IActionResult ViewFarmers(string categoryFilter = "", DateTime? productionDate = null)
        {
            // Check if any filters are applied
            bool isFiltering = !string.IsNullOrEmpty(categoryFilter) || productionDate.HasValue;

            // Load farmers and include their associated products
            var query = _context.Farmers.Include(f => f.Products).AsQueryable();

            // Apply filtering based on provided parameters
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

            // Used in the view to conditionally display a message or results
            ViewBag.IsFiltering = isFiltering;

            return View(farmers);
        }

        // Displays a list of all farmers with options to edit/delete
        public IActionResult ManageFarmers()
        {
            var farmers = _context.Farmers.ToList();
            return View(farmers);
        }

        // GET: Displays the form to edit a farmer's details
        [HttpGet]
        public IActionResult EditFarmer(int id)
        {
            var farmer = _context.Farmers.Find(id);
            if (farmer == null) return NotFound();

            return View(farmer);
        }

        // POST: Updates the farmer details in the database
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

        // GET: Deletes a farmer by ID
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








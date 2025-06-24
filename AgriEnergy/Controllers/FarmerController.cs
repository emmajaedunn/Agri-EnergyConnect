using System;
using System.Linq;
using System.Security.Claims;
using AgriEnergy.Data;
using AgriEnergy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergy.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FarmerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Farmer Dashboard
        public IActionResult FarmerDashboard()
        {
            return View();
        }

        // GET: Show Add Product form
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState)
                {
                    Console.WriteLine($"Key: {modelError.Key}");
                    foreach (var error in modelError.Value.Errors)
                    {
                        Console.WriteLine($"  Error: {error.ErrorMessage}");
                    }
                }

                return View(product);
            }

            // Get the currently logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                ModelState.AddModelError("", "User not logged in.");
                return View(product);
            }

            // Find the farmer linked to this user
            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);
            if (farmer == null)
            {
                ModelState.AddModelError("", "Farmer profile not found.");
                return View(product);
            }

            // Link product to farmer
            product.FarmerId = farmer.Id;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product added successfully!";
            return RedirectToAction("AddProduct");
        }


        // GET: Product Add Success Page
        public IActionResult AddProductSuccess()
        {
            return View();
        }

        // GET: View Products
        public IActionResult ViewProducts(DateTime? productionDate, string category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var farmer = _context.Farmers.FirstOrDefault(f => f.UserId == userId);


            if (farmer == null) return Unauthorized();

            var products = _context.Products
                .Where(p => p.FarmerId == farmer.Id);

            if (productionDate.HasValue)
            {
                var date = productionDate.Value.Date;
                products = products.Where(p => p.ProductionDate.Date == date);
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.ProductCategory.Contains(category));
            }

            return View(products.ToList());
        }

       
        // GET: Manage Products
        public IActionResult ManageProducts()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var farmer = _context.Farmers.FirstOrDefault(f => f.Email == email);

            if (farmer == null) return Unauthorized();

            var products = _context.Products
                .Where(p => p.FarmerId == farmer.Id)
                .ToList();

            return View(products);
        }

        // GET: Edit Product
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var farmer = _context.Farmers.FirstOrDefault(f => f.Email == email);

            if (farmer == null) return Unauthorized();

            var product = _context.Products.FirstOrDefault(p => p.ProductID == id && p.FarmerId == farmer.Id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Edit Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProduct(Product product)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var farmer = _context.Farmers.FirstOrDefault(f => f.Email == email);

            if (farmer == null) return Unauthorized();

            var productInDb = _context.Products.FirstOrDefault(p => p.ProductID == product.ProductID && p.FarmerId == farmer.Id);
            if (productInDb == null) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Update only editable fields manually
            productInDb.ProductName = product.ProductName;
            productInDb.ProductCategory = product.ProductCategory;
            productInDb.ProductionDate = product.ProductionDate;

            // FarmerId stays unchanged
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction("ManageProducts");
        }


        // GET: Delete Product
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var farmer = _context.Farmers.FirstOrDefault(f => f.Email == email);

            if (farmer == null) return Unauthorized();

            var product = _context.Products.FirstOrDefault(p => p.ProductID == id && p.FarmerId == farmer.Id);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageProducts");
        }
    }
}

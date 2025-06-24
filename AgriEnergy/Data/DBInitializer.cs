using AgriEnergy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergy.Data
{
    public static class DBInitializer
    {
        // Main method to seed roles, users, and data
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            // Resolve required services from the dependency injection container
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply any pending migrations
            context.Database.Migrate();

            // Ensure that required roles exist in the database
            string[] roles = { "Farmer", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Farmer and Employee users
            await SeedUsersAndFarmers(context, userManager, roleManager);
        }

        // Seeds demo users and their corresponding data (Farmer and Products)
        public static async Task SeedUsersAndFarmers(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!context.Farmers.Any(f => f.Email == "john@farm.co.za"))
            {
                // Create the ApplicationUser for the farmer
                var user = new ApplicationUser
                {
                    UserName = "john@farm.co.za",
                    Email = "john@farm.co.za",
                    EmailConfirmed = true
                };

                // Create user in the identity system with a default password
                var userResult = await userManager.CreateAsync(user, "Farm123!");
                if (!userResult.Succeeded)
                {
                    return;
                }

                // Assign the "Farmer" role to this user
                var roleResult = await userManager.AddToRoleAsync(user, "Farmer");
                if (!roleResult.Succeeded)
                {
                    return;
                }

                // Create and link the Farmer domain model with the ApplicationUser
                var farmer = new Farmer
                {
                    FullName = "John Maseko",
                    Email = user.Email,
                    ContactNumber = "0721234567",
                    Location = "Limpopo",
                    UserId = user.Id,
                    ApplicationUser = user
                };

                // Add the Farmer to the context and save
                context.Farmers.Add(farmer);
                await context.SaveChangesAsync();

                // Seed Products linked to this farmer
                context.Products.AddRange(
                    new Product
                    {
                        ProductName = "Solar Water Pump",
                        ProductCategory = "Irrigation",
                        ProductionDate = DateTime.Now.AddDays(-30),
                        FarmerId = farmer.Id
                    },
                    new Product
                    {
                        ProductName = "Organic Maize",
                        ProductCategory = "Crop",
                        ProductionDate = DateTime.Now.AddDays(-10),
                        FarmerId = farmer.Id
                    }
                );

                await context.SaveChangesAsync();
            }

            if (await userManager.FindByEmailAsync("employee@agrienergy.com") == null)
            {
                // Create the ApplicationUser for the employee
                var employeeUser = new ApplicationUser
                {
                    UserName = "employee@agrienergy.com",
                    Email = "employee@agrienergy.com",
                    EmailConfirmed = true
                };
                
                // Create the employee user with a default password
                var employeeResult = await userManager.CreateAsync(employeeUser, "Employee123!");
                if (employeeResult.Succeeded)
                {
                    // Assign the "Employee" role
                    await userManager.AddToRoleAsync(employeeUser, "Employee");
                }
            }
        }
    }
}
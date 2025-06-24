using AgriEnergy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergy.Data
{
    public static class DBInitializer
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply any pending migrations
            context.Database.Migrate();

            // Seed roles
            string[] roles = { "Farmer", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed Farmer and Employee users
            await SeedUsersAndFarmers(context, userManager, roleManager);
        }

        // Separate method for seeding users and farmers
        public static async Task SeedUsersAndFarmers(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!context.Farmers.Any(f => f.Email == "john@farm.co.za"))
            {
                // Create ApplicationUser properly
                var user = new ApplicationUser
                {
                    UserName = "john@farm.co.za",
                    Email = "john@farm.co.za",
                    EmailConfirmed = true
                };

                var userResult = await userManager.CreateAsync(user, "Farm123!");
                if (!userResult.Succeeded)
                {
                    // You can log errors here if needed
                    return;
                }

                var roleResult = await userManager.AddToRoleAsync(user, "Farmer");
                if (!roleResult.Succeeded)
                {
                    // Handle role assignment failure if needed
                    return;
                }

                // Now create Farmer with UserId and link ApplicationUser navigation property
                var farmer = new Farmer
                {
                    FullName = "John Maseko",
                    Email = user.Email,
                    ContactNumber = "0721234567",
                    Location = "Limpopo",
                    UserId = user.Id,
                    ApplicationUser = user
                };

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
                var employeeUser = new ApplicationUser
                {
                    UserName = "employee@agrienergy.com",
                    Email = "employee@agrienergy.com",
                    EmailConfirmed = true
                };
                var employeeResult = await userManager.CreateAsync(employeeUser, "Employee123!");
                if (employeeResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser, "Employee");
                }
            }
        }
    }
}
































/*namespace AgriEnergy.Data
{
    public static class DBInitializer
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply any pending migrations
            context.Database.Migrate();

            // Seed roles
            string[] roles = { "Farmer", "Employee" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Farmer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Farmer"));
            }

            if (!context.Farmers.Any(f => f.Email == "john@farm.co.za"))
            {
                var user = new ApplicationUser
                {
                    UserName = "john@farm.co.za",
                    Email = "john@farm.co.za",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Farm123!");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"User creation error: {error.Description}");
                    }
                    return; // stop seeding if user creation fails
                }

                await userManager.AddToRoleAsync(user, "Farmer");

                var farmer = new Farmer
                {
                    FullName = "John Maseko",
                    Email = user.Email,
                    ContactNumber = "0721234567",
                    Location = "Limpopo",
                    UserId = user.Id
                };

                context.Farmers.Add(farmer);
                await context.SaveChangesAsync();

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
        }

            // Seed a farmer user
            // Seed a farmer user
            /*if (!context.Farmers.Any())
            {
                // Step 1: Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = "john@farm.co.za",
                    Email = "john@farm.co.za",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Farm123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Farmer");

                    // Step 2: Now create Farmer using user.Id
                    var farmer = new Farmer
                    {
                        FullName = "John Maseko",
                        Email = user.Email,
                        ContactNumber = "0721234567",
                        Location = "Limpopo",
                        UserId = user.Id // ✅ Set foreign key here
                    };

                    context.Farmers.Add(farmer);
                    await context.SaveChangesAsync();

                    // Step 3: Seed Products
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
            }

            // Seed an employee user
            if (await userManager.FindByEmailAsync("employee@agrienergy.com") == null)
            {
                var employeeUser = new ApplicationUser
                {
                    UserName = "employee@agrienergy.com",
                    Email = "employee@agrienergy.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(employeeUser, "Employee123!");
                await userManager.AddToRoleAsync(employeeUser, "Employee");
            }
        }
    }
}*/

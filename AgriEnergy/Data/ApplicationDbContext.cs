using AgriEnergy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergy.Data
{
    // Inherits from IdentityDbContext to integrate ASP.NET Core Identity with ApplicationUser
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Constructor that accepts DbContext options and passes them to the base class
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // DbSets for your application entities
        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }

        // Configure relationships and additional constraints
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Calls the base method to ensure Identity tables are configured
            base.OnModelCreating(builder);

            // Configure one-to-one relationship between ApplicationUser and Farmer
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Farmer)
                .WithOne(f => f.ApplicationUser)
                .HasForeignKey<Farmer>(f => f.UserId)
                .IsRequired();

            // Ensure that each Farmer's UserId is unique
            builder.Entity<Farmer>()
                .HasIndex(f => f.UserId)
                .IsUnique();
        }
    }
}

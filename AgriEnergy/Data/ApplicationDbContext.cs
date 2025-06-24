using AgriEnergy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriEnergy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Farmer> Farmers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Farmer)
                .WithOne(f => f.ApplicationUser)
                .HasForeignKey<Farmer>(f => f.UserId)
                .IsRequired(); // Optional: make it false if you're okay with null UserId

            // Optional: Make UserId unique if not already
            builder.Entity<Farmer>()
                .HasIndex(f => f.UserId)
                .IsUnique();
        }





    }
}

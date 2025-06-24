using System.ComponentModel.DataAnnotations;

namespace AgriEnergy.Models
{
    public class Farmer
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Location { get; set; }

        // ✅ These two should NOT be required, because they are set server-side
        public string? UserId { get; set; } 

        public ApplicationUser? ApplicationUser { get; set; } 

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}



using System.ComponentModel.DataAnnotations;

namespace AgriEnergy.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}

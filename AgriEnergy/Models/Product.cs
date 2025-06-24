using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergy.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Product category is required")]
        public string ProductCategory { get; set; } = null!;

        [Required(ErrorMessage = "Production date is required")]
        [DataType(DataType.Date)]
        public DateTime ProductionDate { get; set; }

        public int FarmerId { get; set; }

        [NotMapped] 
        public Farmer? Farmer { get; set; }
    }
}



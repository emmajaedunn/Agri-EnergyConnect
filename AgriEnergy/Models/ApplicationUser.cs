using AgriEnergy.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public Farmer Farmer { get; set; }
}


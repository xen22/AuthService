using Microsoft.AspNetCore.Identity;

namespace AD.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
    }
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AD.IdentityModels
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
    }
}
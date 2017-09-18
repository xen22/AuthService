using System.Threading.Tasks;
using AD.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AD.IdentityManager
{
    internal class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new System.ArgumentNullException(nameof(roleManager));
        }

        public async Task<IdentityResult> CreateUser(string username, string password, bool isAdmin)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                IsAdmin = isAdmin
            };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return result;
            }

            var roleName = isAdmin ? "admin" : "user";
            result = await CreateRoleIfNotExists(roleName);

            if (!result.Succeeded)
            {
                return result;
            }

            result = await _userManager.AddToRoleAsync(user, roleName);
            return result;
        }

        public async Task<IdentityResult> CreateRoleIfNotExists(string roleName)
        {
            var result = new IdentityResult();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole();
                role.Name = roleName;
                result = await _roleManager.CreateAsync(role);
            }
            return result;
        }

    }

}
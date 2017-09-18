using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AD.IdentityManager
{
    internal interface IUserService
    {
        Task<IdentityResult> CreateUser(string username, string password, bool isAdmin);
        Task<IdentityResult> CreateRoleIfNotExists(string roleName);
    }
}
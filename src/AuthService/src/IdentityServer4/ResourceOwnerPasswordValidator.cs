using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using AD.IdentityModels;

namespace AD.AuthService.IdentityServer4
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public ResourceOwnerPasswordValidator(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new System.ArgumentNullException(nameof(signInManager));
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            System.Console.WriteLine($"Username: {context.UserName}. Password: {context.Password}");

            var user = await _userManager.FindByNameAsync(context.UserName);

            if (user == null)
            {
                context.Result = new GrantValidationResult(
                    error: TokenRequestErrors.InvalidGrant,
                    errorDescription: "could not retrieve user"
                );
            }
            else
            {

                //var signInResult = await _signInManager.CheckPasswordSignInAsync(user, context.Password, lockoutOnFailure: true);

                //if (signInResult.Succeeded)
                if (context.Password == user.PasswordHash)
                {
                    context.Result = new GrantValidationResult(
                        subject: user.Id,
                        authenticationMethod: "custom"
                    //claims: optionalClaims
                    );
                }
                else
                {
                    context.Result = new GrantValidationResult(
                        error: TokenRequestErrors.InvalidGrant,
                        errorDescription: "invalid custom credential"
                    );
                }
            }
        }
    }
}
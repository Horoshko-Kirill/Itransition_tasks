using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApp.Models;

namespace MyApp.Filtres
{
    public class CheckBlockedAttribute : ActionFilterAttribute
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public CheckBlockedAttribute(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userPrincipal = context.HttpContext.User;

            if (userPrincipal.Identity?.IsAuthenticated == true)
            {
                var appUser = await _userManager.GetUserAsync(userPrincipal);

                if (appUser == null)
                {
                    await _signInManager.SignOutAsync();
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                    return;
                }

                if (appUser.IsBlocked)
                {
                    await _signInManager.SignOutAsync();
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                    return;
                }

                appUser.LastVisit = DateTime.UtcNow;
                await _userManager.UpdateAsync(appUser);
            }

            await next();
        }
    }
}

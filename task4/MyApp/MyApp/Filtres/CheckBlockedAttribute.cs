using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApp.Models;

namespace MyApp.Filtres
{
    public class CheckBlockedAttribute : ActionFilterAttribute
    {

        private readonly UserManager<User> _userManager;


        public CheckBlockedAttribute(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                var appUser = await _userManager.GetUserAsync(user);

                appUser.LastVisit = DateTime.UtcNow;
                await _userManager.UpdateAsync(appUser);

                if (appUser != null && appUser.IsBlocked)
                {
                    var signInManager = context.HttpContext.RequestServices.GetService<SignInManager<User>>();
                    await signInManager.SignOutAsync();
                    context.Result = new RedirectToActionResult("Blocked", "Account", null);
                    return;
                }
            }

            await next();
        }

    }
}

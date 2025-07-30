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
            try
            {
                var user = context.HttpContext.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    var appUser = await _userManager.GetUserAsync(user);

                    if (appUser != null)
                    {
                        appUser.LastVisit = DateTime.UtcNow;
                        await _userManager.UpdateAsync(appUser);

                        if (appUser.IsBlocked)
                        {
                            var signInManager = context.HttpContext.RequestServices.GetService<SignInManager<User>>();
                            if (signInManager != null)
                            {
                                await signInManager.SignOutAsync();
                            }
                            context.Result = new RedirectToActionResult("Blocked", "Account", null);
                            return;
                        }
                    }
                }

                await next();
            }
            catch (Exception ex)
            {
                context.Result = new StatusCodeResult(500);
            }
        }
    }
}

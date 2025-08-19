using CourseWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CourseWork.Filtres
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

                if (appUser.IsBlocked)
                {
                    await _signInManager.SignOutAsync();
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                    return;
                }

                await _signInManager.RefreshSignInAsync(appUser);

                var controllerName = context.RouteData.Values["controller"]?.ToString();
                var actionName = context.RouteData.Values["action"]?.ToString();

                if (IsAdminPage(controllerName, actionName) && !await _userManager.IsInRoleAsync(appUser, "Admin"))
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                    return;
                }

                appUser.LastVisit = DateTime.UtcNow;
                await _userManager.UpdateAsync(appUser);
            }

            await next();
        }

        private bool IsAdminPage(string controllerName, string actionName)
        {

            var adminControllers = new[] { "Admin" };
            var adminActions = new[] { "AdminPage", "ControlUserAction" };

            return adminControllers.Contains(controllerName) || adminActions.Contains(actionName);
        }
    }
}

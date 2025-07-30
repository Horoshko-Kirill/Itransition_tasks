using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;

namespace MyApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(AppDbContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.IsBlocked == true)
            {
                await _signInManager.SignOutAsync();
                TempData["Blocked"] = "You have been blocked.";
                return RedirectToAction("Login", "Account");
            }

            var users = await _context.Users
                             .OrderBy(u => u.Name)
                             .ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> BulkAction(string action, string[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
            {
                TempData["Message"] = "No users selected.";
                return RedirectToAction("Index");
            }

            var currentUserId = _userManager.GetUserId(User);

            foreach (var id in selectedIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) continue;

                switch (action)
                {
                    case "block":
                        user.IsBlocked = true;
                        await _userManager.UpdateAsync(user);


                        if (user.Id == currentUserId)
                            await _signInManager.SignOutAsync();
                        break;

                    case "unblock":
                        user.IsBlocked = false;
                        await _userManager.UpdateAsync(user);
                        break;

                    case "delete":
                        await _userManager.DeleteAsync(user);

                        if (user.Id == currentUserId)
                            await _signInManager.SignOutAsync();
                        break;

                }
 
            }

            TempData["Message"] = $"Action '{action}' completed.";
            return RedirectToAction("Index");
        }
    }
}

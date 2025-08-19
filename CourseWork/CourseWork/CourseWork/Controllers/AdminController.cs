using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CourseWork.Data;
using CourseWork.Models;
using Microsoft.EntityFrameworkCore;
using CourseWork.Services;
using Dropbox.Api.Team;

namespace CourseWork.Controllers
{

    [Authorize]
    public class AdminController : Controller
    {

        private readonly CourseWorkDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DropboxService _dropboxService;

        public AdminController(CourseWorkDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, DropboxService dropboxService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _dropboxService = dropboxService;
        }

        public async Task<IActionResult> AdminPage()
        {

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.IsBlocked == true)
            {
                await _signInManager.SignOutAsync();
                TempData["Blocked"] = "You have been blocked.";
                return RedirectToAction("Login", "Account");
            }

            var users = await _context.Users
                              .OrderBy(u => u.UserName)
                              .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ControlUserAction(string action, string[] selectedIds)
        {

            if (selectedIds == null || selectedIds.Length == 0)
            {
                TempData["Message"] = "No users selected.";
                return RedirectToAction("AdminPage");
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
                        await _dropboxService.DeleteFileAsync(user.DropboxPath);
                        await _userManager.DeleteAsync(user);

                        if (user.Id == currentUserId)
                            await _signInManager.SignOutAsync();
                        break;

                    case "give_admin":
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, "Admin");
                        await _signInManager.RefreshSignInAsync(user);
                        break;

                    case "remove_admin":
                        var rolesToRemove = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                        await _userManager.AddToRoleAsync(user, "User");
                        await _signInManager.RefreshSignInAsync(user);
                        if (user.Id == currentUserId)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        break;
                }
            }
            
            TempData["Message"] = $"Action '{action}' completed.";
            return RedirectToAction("AdminPage");
        }
    }
}

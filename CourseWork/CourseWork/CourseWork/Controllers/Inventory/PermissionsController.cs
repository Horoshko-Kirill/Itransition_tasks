using CourseWork.Data;
using CourseWork.Migrations;
using CourseWork.Models;
using CourseWork.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers.Inventory
{
    public class PermissionsController : InventoryBaseController
    {

        public PermissionsController(CourseWorkDbContext context, UserManager<User> userManager) : base(context, userManager) 
        {
        }

        [HttpGet]
        public async Task<IActionResult> Permissions(int inventoryId)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }

            SetInventoryViewData(inventoryId, inventory.Name);

            if (inventory == null)
                return NotFound();
            ViewBag.isPublic = inventory.isPublic;
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");


            var users = await _userManager.Users
                .Where(u => u.Id != inventory.CreatorId)
                .ToListAsync();

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            users = users.Where(u => !admins.Any(a => a.Id == u.Id)).ToList();

            var model = new PermissionsViewModel
            {
                InventoryId = inventoryId,
                Users = users.Select(u => new UserPermissionViewModel
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    HaveWriteAccess = inventory.Permissions.Any(p => p.UserId == u.Id && p.HaveWriteAccess)
                }).ToList()
            };

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return View("~/Views/Inventory/Permissions/Permissions.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Permissions(PermissionsViewModel model)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == model.InventoryId);
            ViewBag.isPublic = inventory.isPublic;
            if (inventory == null)
                return NotFound();

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }

            SetInventoryViewData(model.InventoryId, inventory.Name);

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && inventory.CreatorId != currentUserId)
                return Forbid();

            foreach (var userModel in model.Users)
            {
                var permission = inventory.Permissions.FirstOrDefault(p => p.UserId == userModel.UserId);

                if (permission != null)
                {

                    permission.HaveWriteAccess = userModel.HaveWriteAccess;
                    _context.Permissions.Update(permission);
                }
                else if (userModel.HaveWriteAccess)
                {

                    _context.Permissions.Add(new Permission
                    {
                        InventoryId = inventory.Id,
                        UserId = userModel.UserId,
                        HaveWriteAccess = true
                    });
                }
            }

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            TempData["SuccessMessage"] = "Permissions updated successfully!";
            return RedirectToAction("Permissions", new { inventoryId = model.InventoryId });
        }

    }
}

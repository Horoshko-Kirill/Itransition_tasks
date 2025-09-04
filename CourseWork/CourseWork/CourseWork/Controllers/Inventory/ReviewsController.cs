using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace CourseWork.Controllers.Inventory
{
    public class ReviewsController : InventoryBaseController
    {

        public ReviewsController(CourseWorkDbContext context, UserManager<User> userManager) : base(context, userManager) { }


        [HttpGet]
        public async Task<IActionResult> Reviews(int inventoryId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.InventoryId == inventoryId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            SetInventoryViewData(inventoryId, inventory.Name);

            ViewBag.isPublic = inventory.isPublic;

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return View("~/Views/Inventory/Reviews/Reviews.cshtml", reviews);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(int inventoryId, string content, int rating)
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

            ViewBag.isPublic = inventory.isPublic;

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            var review = new Review
            {
                Content = content,
                Reating = rating,
                InventoryId = inventoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Reviews", new { inventoryId });
        }

    }
}

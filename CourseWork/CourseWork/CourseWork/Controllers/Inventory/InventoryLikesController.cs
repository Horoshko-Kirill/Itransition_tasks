using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers.Inventory
{
    public class InventoryLikesController : InventoryBaseController
    {

        public InventoryLikesController(CourseWorkDbContext context, UserManager<User> userManager)
            : base(context, userManager) { }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleLike(int inventoryId)
        {
            var userId = _userManager.GetUserId(User);
            var like = await _context.InventoryLikes
                .FirstOrDefaultAsync(l => l.inventoryId == inventoryId && l.UserId == userId);

            if (like != null)
            {
                _context.InventoryLikes.Remove(like);
            }
            else
            {
                _context.InventoryLikes.Add(new InventoryLike
                {
                    inventoryId = inventoryId,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Description", "Description", new { inventoryId });
        }

    }
}

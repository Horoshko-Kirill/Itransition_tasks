using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers.Inventory
{
    public class InventoryBaseController : Controller
    {

        protected readonly CourseWorkDbContext _context;
        protected readonly UserManager<User> _userManager;

        public InventoryBaseController(CourseWorkDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected async Task<bool> ChekInventoryAccess(int inventoryId)
        {

            var inventory = await _context.Inventories
                .Include(i => i.Creator)
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            if (inventory == null)
            {
                return false;
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return false;
            }

            if (inventory.CreatorId == user.Id)
            {
                ViewBag.IsCreator = true;
                return true;
            }

            if (User.IsInRole("Admin"))
            {
                ViewBag.IsCreator = true;
                return true;
            }

            bool hasAccess = inventory.Permissions.Any(p => p.UserId == user.Id && p.HaveWriteAccess);

            if (hasAccess)
            {
                ViewBag.IsCreator = false;
                return true;
            }

            return false;
        }

        protected void SetInventoryViewData(int inventoryId, string inventoryName)
        {

            ViewBag.InventoryId = inventoryId;
            ViewBag.InventoryName = inventoryName;

        }
    }
}

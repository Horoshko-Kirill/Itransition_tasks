using CourseWork.Data;
using CourseWork.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers.Inventory
{
    public class ItemsController : InventoryBaseController
    {

        public ItemsController(CourseWorkDbContext context, UserManager<User> userManager) : base(context, userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Items(int inventoryId)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            SetInventoryViewData(inventoryId, inventory.Name);

            var items = await _context.Items.ToListAsync();

            return View(items);

        }
    }
}

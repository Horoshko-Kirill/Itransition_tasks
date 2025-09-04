using CourseWork.Data;
using CourseWork.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers
{
    public class HomeController : Controller
    {

        private readonly CourseWorkDbContext _context;

        public HomeController(CourseWorkDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            var popularInventories = await _context.InventoryLikes
                .Include(l => l.Inventory)
                    .ThenInclude(i => i.Category)
                .Include(l => l.Inventory)
                    .ThenInclude(i => i.InventoryTags)
                        .ThenInclude(it => it.Tag)
                .GroupBy(l => l.inventoryId)
                .Select(g => new InventoryWithLikesViewModel
                {
                    Inventory = g.First().Inventory,
                    LikesCount = g.Count()
                })
                .OrderByDescending(x => x.LikesCount)
                .Take(5)
                .ToListAsync();

            var latestInventories = await _context.Inventories
                .Include(i => i.Category)
                .Include(i => i.InventoryTags)
                    .ThenInclude(it => it.Tag)
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToListAsync();

            var model = new HomeViewModel
            {
                PopularInventories = popularInventories,
                LatestInventories = latestInventories
            };

            return View(model);
        }
    }
}

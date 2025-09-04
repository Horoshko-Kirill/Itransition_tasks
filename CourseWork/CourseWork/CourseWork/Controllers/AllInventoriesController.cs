using CourseWork.Data;
using CourseWork.Models;
using CourseWork.Models.ViewModels;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers
{
    public class AllInventoriesController : Controller
    {

        private readonly CourseWorkDbContext _context;

        public AllInventoriesController(CourseWorkDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AllInventories(string searchQuery, int? categoryId, int? tagId)
        {
            var query = _context.Inventories
                .Include(i => i.Category)
                .Include(i => i.InventoryTags)
                    .ThenInclude(it => it.Tag)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(i => i.Name.Contains(searchQuery));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }

            if (tagId.HasValue)
            {
                if (tagId.Value == 0) 
                {
                    query = query.Where(i => !i.InventoryTags.Any());
                }
                else
                {
                    query = query.Where(i => i.InventoryTags.Any(it => it.TagId == tagId.Value));
                }
            }

            var model = new InventoryListViewModel
            {
                Inventories = await query.ToListAsync(),
                SearchQuery = searchQuery,
                SelectedCategoryId = categoryId,
                SelectedTagId = tagId,
                CategoryOptions = new SelectList(await _context.Categories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name"),
                TagOptions = await _context.Tags.OrderBy(t => t.Name).ToListAsync()
            };

            return View(model);
        }



    }
}

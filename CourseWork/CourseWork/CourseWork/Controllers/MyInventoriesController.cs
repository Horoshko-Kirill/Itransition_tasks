using CourseWork.Data;
using CourseWork.Models;
using CourseWork.Models.ViewModels;
using CourseWork.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers
{
    [Authorize]
    public class MyInventoriesController : Controller
    {

        private readonly CourseWorkDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly DropboxService _dropboxService;


        public MyInventoriesController(CourseWorkDbContext context, UserManager<User> userManager, DropboxService dropboxService)
        {
            _context = context;
            _userManager = userManager;
            _dropboxService = dropboxService;
        }

        public async Task<IActionResult> MyInventories()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = await GetUserInventories(user.Id);

            ViewBag.Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var model = new InventoryCreateViewModel();
            await LoadCategoryOptions(model);

            return View(model);

        }


        [HttpPost]
        public async Task<IActionResult> Create(InventoryCreateViewModel model)
        {
            ModelState.Remove("CategoryOptions");

            if (model.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == model.CategoryId.Value);

                if (!categoryExists)
                {
                    ModelState.AddModelError("CategoryId", "Selected category does not exist");
                }
            }

            if (!ModelState.IsValid)
            {

                await LoadCategoryOptions(model);
                return View(model);

            }

            var user = await _userManager.GetUserAsync(User);

            var inventory = new CourseWork.Models.Inventory
            {
                Name = model.Name,
                Description = model.Description,
                CategoryId = model.CategoryId,
                isPublic = model.IsPublic,
                CreatorId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using var stream = model.ImageFile.OpenReadStream();
                    string url = await _dropboxService.UploadInventoryImageAsync(
                        stream,
                        model.ImageFile.FileName,
                        inventory.Id.ToString());

                    string newPath = $"/inventories/{inventory.Id}_image_{model.ImageFile.FileName}";

                    inventory.ImageUrl = url;
                    inventory.ImageDropboxPath = newPath;
                }

                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyInventories");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating inventory: {ex.Message}";
                await LoadCategoryOptions(model);
                return View(model);
            }

        }

        private async Task LoadCategoryOptions(InventoryCreateViewModel model)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            model.CategoryOptions = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == model.CategoryId
            }).ToList();

        }

        private async Task<MyInventoriesViewModel> GetUserInventories(string userId)
        {
            var createdInventories = await _context.Inventories
               .Include(i => i.Category)
               .Include(i => i.Creator)
               .Include(i => i.Items)
               .Where(i => i.CreatorId == userId)
               .OrderByDescending(i => i.CreatedAt)
               .ToListAsync();

           
            var accessibleInventories = await _context.Inventories
                .Include(i => i.Category)
                .Include(i => i.Creator)
                .Include(i => i.Items)
                .Include(i => i.Permissions)
                .Where(i => i.Permissions.Any(p => p.UserId == userId && p.HaveWriteAccess) && i.CreatorId != userId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var publicInventories = await _context.Inventories
                .Include(i => i.Category)
                .Include(i => i.Creator)
                .Include(i => i.Items)
                .Where(i => i.isPublic)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return new MyInventoriesViewModel
            {
                CreatedInventories = createdInventories,
                AccessibleInventories = accessibleInventories,
                PublicInventories = publicInventories,
                CurrentUserId = userId
            };
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(int[] selectedInventories)
        {
            if (selectedInventories == null ||  selectedInventories.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select at least one inventory to delete";
                return RedirectToAction("MyInventories");
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = User.IsInRole("Admin");

            foreach (var inventoryId in selectedInventories)
            {

                var inventory = await _context.Inventories
                   .Include(i => i.CustomIdFormat)
                   .FirstOrDefaultAsync(i => i.Id == inventoryId);

                if (inventory == null) continue;

                if (inventory.CreatorId != user.Id && !isAdmin)
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete some selected inventories";
                    return RedirectToAction("MyInventories");
                }

                if (!string.IsNullOrEmpty(inventory.ImageDropboxPath))
                {
                    await _dropboxService.DeleteFileAsync(inventory.ImageDropboxPath);
                }

                _context.Inventories.Remove(inventory);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("MyInventories");
        }
    }
}

using CourseWork.Data;
using CourseWork.Models;
using CourseWork.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Controllers.Inventory
{
    public class DescriptionController : InventoryBaseController
    {

        private readonly DropboxService _dropboxService;
        public DescriptionController(
           CourseWorkDbContext context,
           UserManager<User> userManager,
           DropboxService dropboxService) : base(context, userManager)
        {
            _dropboxService = dropboxService;
        }

        [HttpGet]
        public async Task<IActionResult> Description(int inventoryId)
        {
            var inventory = await _context.Inventories
               .Include(i => i.Permissions)
               .Include(i => i.InventoryTags)
                    .ThenInclude(it => it.Tag)
               .FirstOrDefaultAsync(i => i.Id == inventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            SetInventoryViewData(inventoryId, inventory.Name);

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            ViewBag.TagString = string.Join(", ", inventory.InventoryTags.Select(it => it.Tag.Name));

            ViewBag.LikeCount = await _context.InventoryLikes
                .CountAsync(l => l.inventoryId == inventory.Id);

            ViewBag.UserLiked = await _context.InventoryLikes
                .AnyAsync(l => l.inventoryId == inventory.Id && l.UserId == userId);

            return View("~/Views/Inventory/Description/Description.cshtml", inventory);
        }

        [HttpGet]
        public async Task<IActionResult> EditDescription(int inventoryId)
        {
            var inventory = await _context.Inventories
               .Include(i => i.Permissions)
               .Include(i => i.InventoryTags)
                    .ThenInclude(it => it.Tag)
               .FirstOrDefaultAsync(i => i.Id == inventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            SetInventoryViewData(inventoryId, inventory.Name);

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            ViewBag.TagString = string.Join(", ", inventory.InventoryTags.Select(it => it.Tag.Name));

            return View("~/Views/Inventory/Description/EditDescription.cshtml", inventory);
        }

        [HttpPost]
        public async Task<IActionResult> EditDescription(int inventoryId, string name, string description, IFormFile? imageFile, string tags)
        {

            var inventory = await _context.Inventories
               .Include(i => i.Permissions)
               .Include(i => i.InventoryTags)
                    .ThenInclude(it => it.Tag)
               .FirstOrDefaultAsync(i => i.Id == inventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            SetInventoryViewData(inventoryId, inventory.Name);

            inventory.Name = name;
            inventory.Description = description;
            inventory.UpdatedAt = DateTime.UtcNow;

            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(inventory.ImageDropboxPath))
                    await _dropboxService.DeleteFileAsync(inventory.ImageDropboxPath);

                using var stream = imageFile.OpenReadStream();
                string url = await _dropboxService.UploadInventoryImageAsync(
                    stream,
                    imageFile.FileName,
                    inventory.Id.ToString());

                string newPath = $"/inventories/{inventory.Id}_image_{imageFile.FileName}";

                inventory.ImageUrl = url;
                inventory.ImageDropboxPath = newPath;
            }

            var tagNames = tags?
               .Split(',', StringSplitOptions.RemoveEmptyEntries)
               .Select(t => t.Trim().ToLower())
               .Distinct()
               .ToList() ?? new List<string>();

            _context.inventoryTags.RemoveRange(inventory.InventoryTags);

            foreach (var tagName in tagNames)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName,
                        CreatedDate = DateTime.UtcNow
                    };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                inventory.InventoryTags.Add(new InventoryTag
                {
                    TagId = tag.Id,
                    CreatedAt = DateTime.UtcNow
                });
            }


            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return RedirectToAction("Description", new { inventoryId = inventory.Id });
        }


    }
}

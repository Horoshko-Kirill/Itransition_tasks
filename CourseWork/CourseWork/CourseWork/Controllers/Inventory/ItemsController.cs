using CourseWork.Data;
using CourseWork.Migrations;
using CourseWork.Models;
using CourseWork.Models.ViewModels;
using CourseWork.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Dropbox.Api.Files.ListRevisionsMode;

namespace CourseWork.Controllers.Inventory
{
    public class ItemsController : InventoryBaseController
    {
        private readonly CustomIdService _customIdService;
        private readonly DropboxService _dropboxService;

        public ItemsController(
            CourseWorkDbContext context,
            UserManager<User> userManager,
            CustomIdService customIdService,
            DropboxService dropboxService
        ) : base(context, userManager)
        {
            _customIdService = customIdService;
            _dropboxService = dropboxService;
        }

        [HttpGet]
        public async Task<IActionResult> Items(int inventoryId)
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
            var items = await _context.Items
                .Include(i => i.CustomFieldValues)
                .ThenInclude(cfv => cfv.CustomField)
                .Where(i => i.InventoryId == inventoryId)
                .ToListAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return View("~/Views/Inventory/Item/Items.cshtml", items);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int inventoryId)
        {
            ViewBag.InventoryId = inventoryId;

            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);
            SetInventoryViewData(inventoryId, inventory.Name);
            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            var customFields = await _context.CustomFields
                .Where(cf => cf.InventoryId == inventoryId)
                .OrderBy(cf => cf.DisplayOrder)
                .ToListAsync();

            var item = new Item
            {
                InventoryId = inventoryId,
                CustomFieldValues = customFields.Select(cf => new CustomFieldValue
                {
                    CustomFieldId = cf.Id,
                    CustomField = cf
                }).ToList()
            };

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return View("~/Views/Inventory/Item/Create.cshtml", item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Item item, IFormFile? imageFile)
        {

            ViewBag.InventoryId = item.InventoryId;

            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == item.InventoryId);

            SetInventoryViewData(item.InventoryId, inventory.Name);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            var format = await _context.CustomIdFormats
                .Include(f => f.Elements)
                .FirstOrDefaultAsync(f => f.InventoryId == item.InventoryId);

            if (format != null)
            {
                int lastSeq = await _context.Items
                    .Where(i => i.InventoryId == item.InventoryId)
                    .CountAsync();

                item.CustomId = _customIdService.Generate(format, lastSeq);
            }


            if (imageFile != null && imageFile.Length > 0)
            {
                using var stream = imageFile.OpenReadStream();
                string imageUrl = await _dropboxService.UploadInventoryItemAsync(
                    stream,
                    imageFile.FileName,
                    item.InventoryId.ToString()
                );
                item.ImageUrl = imageUrl;
                item.ImageDropboxPath = $"/item/{item.InventoryId}_image_{imageFile.FileName}";

            }

            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;


            foreach (var cfv in item.CustomFieldValues)
            {
                cfv.CreatedAt = DateTime.UtcNow;
                cfv.UpdatedAt = DateTime.UtcNow;
                cfv.ItemId = item.Id;
            }

            if (!ModelState.IsValid) { foreach (var kvp in ModelState) { foreach (var error in kvp.Value.Errors) { Console.WriteLine($"Field: {kvp.Key}, Error: {error.ErrorMessage}"); } } return View("~/Views/Inventory/Item/Create.cshtml", item); }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return RedirectToAction("Items", new { inventoryId = item.InventoryId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.Items
                .Include(i => i.CustomFieldValues)
                .ThenInclude(cfv => cfv.CustomField)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound();

            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == item.InventoryId);

            SetInventoryViewData(inventory.Id, inventory.Name);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            var allFields = await _context.CustomFields
                .Where(cf => cf.InventoryId == item.InventoryId)
                .ToListAsync();


            foreach (var cf in allFields)
            {
                if (!item.CustomFieldValues.Any(v => v.CustomFieldId == cf.Id))
                {
                    var defaultValue = cf.FieldType switch
                    {
                        CourseWork.Models.Enums.CustomFieldType.Boolean => "false",
                        CourseWork.Models.Enums.CustomFieldType.Numeric => "0",
                        _ => ""
                    };

                    item.CustomFieldValues.Add(new CustomFieldValue
                    {
                        CustomFieldId = cf.Id,
                        ItemId = item.Id,
                        Value = defaultValue,
                        CustomField = cf
                    });
                }
            }


            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            var format = await _context.CustomIdFormats
             .Include(f => f.Elements)
             .FirstOrDefaultAsync(f => f.InventoryId == item.InventoryId);


            string preview = format.Elements != null
                ? string.Join("--", format.Elements.OrderBy(e => e.Order).Select(e => e.FixedValue))
                : "";

            ViewBag.Preview = preview;

            return View("~/Views/Inventory/Item/Edit.cshtml", item);
        }



        [HttpPost]
        public async Task<IActionResult> Edit(Item item, IFormFile? imageFile)
        {
            ViewBag.InventoryId = item.InventoryId;

            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == item.InventoryId);

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;

            var existingItem = await _context.Items
                .Include(i => i.CustomFieldValues)
                .FirstOrDefaultAsync(i => i.Id == item.Id);

            if (existingItem == null)
                return NotFound();

            var format = await _context.CustomIdFormats
                .Include(f => f.Elements)
                .FirstOrDefaultAsync(f => f.InventoryId == item.InventoryId);

            if (format != null && !_customIdService.Check(item.CustomId, format))
            {
                ModelState.AddModelError("CustomId", "Custom ID is not valid.");
            }

            var allCustomFields = await _context.CustomFields
                .Where(cf => cf.InventoryId == existingItem.InventoryId)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
          
                foreach (var cf in allCustomFields)
                {
                    if (!existingItem.CustomFieldValues.Any(v => v.CustomFieldId == cf.Id))
                    {
                        var defaultValue = cf.FieldType switch
                        {
                            CourseWork.Models.Enums.CustomFieldType.Boolean => "false",
                            CourseWork.Models.Enums.CustomFieldType.Numeric => "0",
                            _ => ""
                        };

                        existingItem.CustomFieldValues.Add(new CustomFieldValue
                        {
                            CustomFieldId = cf.Id,
                            ItemId = existingItem.Id,
                            Value = defaultValue,
                            CustomField = cf
                        });
                    }
                }

                return View("~/Views/Inventory/Item/Edit.cshtml", existingItem);
            }

 
            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.CustomId = item.CustomId;
            existingItem.UpdatedAt = DateTime.UtcNow;


            if (imageFile != null && imageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingItem.ImageDropboxPath))
                    await _dropboxService.DeleteFileAsync(existingItem.ImageDropboxPath);

                using var stream = imageFile.OpenReadStream();
                string imageUrl = await _dropboxService.UploadInventoryItemAsync(
                    stream,
                    imageFile.FileName,
                    item.InventoryId.ToString()
                );

                existingItem.ImageUrl = imageUrl;
                existingItem.ImageDropboxPath = $"/item/{item.InventoryId}_image_{imageFile.FileName}";
            }

 
            foreach (var cf in allCustomFields)
            {
                var submittedValue = item.CustomFieldValues.FirstOrDefault(v => v.CustomFieldId == cf.Id);
                var existingValue = existingItem.CustomFieldValues.FirstOrDefault(v => v.CustomFieldId == cf.Id);

                if (existingValue != null)
                {
                    existingValue.Value = submittedValue?.Value ?? existingValue.Value;
                    existingValue.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    existingItem.CustomFieldValues.Add(new CustomFieldValue
                    {
                        CustomFieldId = cf.Id,
                        ItemId = existingItem.Id,
                        Value = submittedValue?.Value ?? (cf.FieldType == CourseWork.Models.Enums.CustomFieldType.Boolean ? "false" :
                                                         cf.FieldType == CourseWork.Models.Enums.CustomFieldType.Numeric ? "0" : ""),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            ViewBag.CanEdit = inventory.Permissions
                .Any(p => p.UserId == userId && p.HaveWriteAccess);

            return RedirectToAction("Items", new { inventoryId = item.InventoryId });
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            ViewBag.InventoryId = item.InventoryId;

            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == item.InventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            if (item == null) return NotFound();

            if (!string.IsNullOrEmpty(item.ImageDropboxPath))
                await _dropboxService.DeleteFileAsync(item.ImageDropboxPath);

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return RedirectToAction("Items", new { inventoryId = item.InventoryId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(int inventoryId, int[] selectedItemIds)
        {
            var items = await _context.Items
             .Where(i => selectedItemIds.Contains(i.Id))
             .Include(i => i.CustomFieldValues)
             .ToListAsync();

            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            foreach (var item in items)
            {

                if (!string.IsNullOrEmpty(item.ImageDropboxPath))
                    await _dropboxService.DeleteFileAsync(item.ImageDropboxPath);

                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return RedirectToAction("Items", new { inventoryId });
        }

    }
}

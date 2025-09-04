using CourseWork.Data;
using CourseWork.Migrations;
using CourseWork.Models;
using CourseWork.Models.Enums;
using CourseWork.Models.ViewModels;
using Dropbox.Api.TeamLog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Dropbox.Api.Files.ListRevisionsMode;

namespace CourseWork.Controllers.Inventory
{
    public class CustomIdController : InventoryBaseController
    {

        public CustomIdController(CourseWorkDbContext context, UserManager<User> userManager) : base(context, userManager)
        {
        }


        [HttpGet]
        public async Task<IActionResult> CustomId(int inventoryId)
        {
            var format = await _context.CustomIdFormats
              .Include(f => f.Elements)
              .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

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
            if (format == null)
            {
                format = new CustomIdFormat
                {
                    InventoryId = inventoryId,
                    Description = "Custom ID format",
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                    Elements = new List<CustomIdElement>()
                };

                _context.CustomIdFormats.Add(format);
                
                await _context.SaveChangesAsync();
            }
            else
            {
                format.Elements = format.Elements.OrderBy(e => e.Order).ToList();
            }

            var model = new CustomIdFormatViewModel
            {
                Id = format.Id,
                Elements = format.Elements.Select(e => new CustomIdElementViewModel
                {
                    Id = e.Id,
                    Type = e.Type,
                    FixedValue = e.FixedValue,
                    Order = e.Order
                }).ToList()
            };

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return View("~/Views/Inventory/CustomId/CustomId.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {;

            var format = await _context.CustomIdFormats
              .Include(f => f.Elements)
              .FirstOrDefaultAsync(f => f.Id == id);

            var inventory = await _context.Inventories
               .Include(i => i.Permissions)
               .FirstOrDefaultAsync(i => i.Id == format.InventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            SetInventoryViewData(format.InventoryId, inventory.Name);

            var model = new CustomIdFormatViewModel
            {
                Id = format.Id,
                Elements = format.Elements
                    .OrderBy(e => e.Id)
                    .Select((e, i) => new CustomIdElementViewModel()
                    {
                        Id = e.Id,
                        Type = e.Type,
                        FixedValue = e.FixedValue,
                        Order = i
                    }).ToList()
            };

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return View("~/Views/Inventory/CustomId/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomIdFormatViewModel model)
        {

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Field: {kvp.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View("~/Views/Inventory/CustomId/CustomId.cshtml", model); 
            }    
               

            var format = await _context.CustomIdFormats
               .Include(f => f.Elements)
               .FirstOrDefaultAsync(f => f.Id == model.Id);

            var inventory = await _context.Inventories
               .Include(i => i.Permissions)
               .FirstOrDefaultAsync(i => i.Id == format.InventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            SetInventoryViewData(format.InventoryId, inventory.Name);

            format.UpdateAt = DateTime.UtcNow;

            _context.CustomIdElements.RemoveRange(format.Elements);

            format.Elements = model.Elements
                .OrderBy(e => e.Order)
                .Select(e => new CustomIdElement()
                {
                    Type = e.Type,
                    FixedValue = e.FixedValue,
                    Order = e.Order
                }).ToList();


            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return RedirectToAction("CustomId", new { inventoryId = format.InventoryId });

        }
    }
}

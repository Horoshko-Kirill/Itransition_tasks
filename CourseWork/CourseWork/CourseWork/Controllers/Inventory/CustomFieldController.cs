using CourseWork.Data;
using CourseWork.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWork.Models;
using CourseWork.Migrations;

namespace CourseWork.Controllers.Inventory
{
    public class CustomFieldController : InventoryBaseController
    {
        public CustomFieldController(CourseWorkDbContext context, UserManager<User> userManager)
            : base(context, userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int inventoryId)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == inventoryId);

            SetInventoryViewData(inventoryId, inventory.Name);

            var fields = await _context.CustomFields
                .Where(cf => cf.InventoryId == inventoryId)
                .OrderBy(cf => cf.DisplayOrder)
                .ToListAsync();

            var model = new CustomFieldViewModel
            {
                Id = inventoryId,
                Elements = fields.Select((f, i) => new CustomFieldElementViewModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description,
                    Type = f.FieldType,
                    ShowInTableView = f.ShowInTableView,
                    Order = i
                }).ToList()
            };

            return View("~/Views/Inventory/CustomField/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomFieldViewModel model)
        {
            var inventory = await _context.Inventories
                 .FirstOrDefaultAsync(i => i.Id == model.Id);

            SetInventoryViewData(model.Id, inventory.Name);

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Field: {kvp.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View("~/Views/Inventory/CustomField/Edit.cshtml", model);
            }

            var inventoryId = model.Id;

            var existingFields = await _context.CustomFields
                .Where(cf => cf.InventoryId == inventoryId)
                .ToListAsync();

            _context.CustomFields.RemoveRange(existingFields);

            var newFields = model.Elements
                .OrderBy(e => e.Order)
                .Select(e => new CustomField()
                {
                    Name = e.Name,
                    Description = e.Description,
                    FieldType = e.Type,
                    ShowInTableView = e.ShowInTableView,
                    DisplayOrder = e.Order,
                    InventoryId = inventoryId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

            _context.CustomFields.AddRange(newFields);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "CustomField", new { inventoryId = inventoryId, area = "Inventory" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var field = await _context.CustomFields.FindAsync(id);
            if (field == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == field.InventoryId);

            SetInventoryViewData(field.InventoryId, inventory.Name);

            var hasValues = await _context.CustomFieldValues.AnyAsync(cfv => cfv.CustomFieldId == id);
            if (hasValues)
            {
                TempData["ErrorMessage"] = "Cannot delete field that has values. Delete the values first.";
                return RedirectToAction("Edit", "CustomField", new { inventoryId = field.InventoryId, area = "Inventory" });
            }

            _context.CustomFields.Remove(field);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Field deleted successfully.";
            return RedirectToAction("Edit", "CustomField", new { inventoryId = field.InventoryId, area = "Inventory" });
        }

    }
}

using CourseWork.Data;
using CourseWork.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseWork.Models;
using CourseWork.Migrations;
using CourseWork.Models.Enums;


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
               .Include(i => i.Permissions)
               .FirstOrDefaultAsync(i => i.Id == inventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;

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

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            return View("~/Views/Inventory/CustomField/Edit.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomFieldViewModel model)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == model.Id);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            SetInventoryViewData(model.Id, inventory.Name);

            if (!ModelState.IsValid)
                return View("~/Views/Inventory/CustomField/Edit.cshtml", model);


            var existingFields = await _context.CustomFields
                .Include(cf => cf.CustomFieldValues)
                .Where(cf => cf.InventoryId == model.Id)
                .ToListAsync();


            var modelFieldIds = model.Elements.Where(e => e.Id != 0).Select(e => e.Id).ToList();

            var fieldsToDelete = existingFields.Where(f => !modelFieldIds.Contains(f.Id)).ToList();
            foreach (var field in fieldsToDelete)
            {
                if (field.CustomFieldValues.Any())
                    _context.CustomFieldValues.RemoveRange(field.CustomFieldValues);

                _context.CustomFields.Remove(field);
            }

            foreach (var element in model.Elements.OrderBy(e => e.Order))
            {
                if (element.Id == 0)
                {
     
                    var newField = new CustomField
                    {
                        Name = element.Name,
                        Description = element.Description,
                        FieldType = element.Type,
                        ShowInTableView = element.ShowInTableView,
                        DisplayOrder = element.Order,
                        InventoryId = model.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.CustomFields.Add(newField);
                }
                else
                {

                    var existingField = existingFields.First(f => f.Id == element.Id);
                    bool typeChanged = existingField.FieldType != element.Type;

                    existingField.Name = element.Name;
                    existingField.Description = element.Description;
                    existingField.FieldType = element.Type;
                    existingField.ShowInTableView = element.ShowInTableView;
                    existingField.DisplayOrder = element.Order;
                    existingField.UpdatedAt = DateTime.UtcNow;

                    if (typeChanged)
                    {

                        foreach (var value in existingField.CustomFieldValues)
                        {
                            switch (element.Type)
                            {
                                case CustomFieldType.SingleLineText:
                                case CustomFieldType.MultiLineText:
                                    value.Value = "";
                                    break;
                                case CustomFieldType.Numeric:
                                    value.Value = "0";
                                    break;
                                case CustomFieldType.Boolean:
                                    value.Value = "false";
                                    break;
                                case CustomFieldType.DocumentOrImage:
                                    value.Value = "";
                                    break;
                            }
                            value.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    _context.CustomFields.Update(existingField);
                }
            }

            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            TempData["SuccessMessage"] = "Fields saved successfully.";
            return RedirectToAction("Edit", new { inventoryId = model.Id, area = "Inventory" });
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
                .Include(i => i.Permissions)
                .FirstOrDefaultAsync(i => i.Id == field.InventoryId);

            var currnetUserId = _userManager.GetUserId(User);
            if (currnetUserId == inventory.CreatorId)
            {
                ViewBag.IsCreator = true;
            }
            ViewBag.isPublic = inventory.isPublic;
            SetInventoryViewData(field.InventoryId, inventory.Name);

            _context.CustomFieldValues.RemoveRange(field.CustomFieldValues);

            _context.CustomFields.Remove(field);
            await _context.SaveChangesAsync();

            var userId = _userManager.GetUserId(User);
            bool hasWritePermission = inventory.Permissions
             .Any(p => p.UserId == userId && p.HaveWriteAccess);

            ViewBag.CanEdit = hasWritePermission;

            TempData["SuccessMessage"] = "Field deleted successfully.";
            return RedirectToAction("Edit", "CustomField", new { inventoryId = field.InventoryId, area = "Inventory" });
        }

    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseWork.Models.ViewModels
{
    public class InventoryCreateViewModel
    {

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 symbol")]
        public string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Description must not 2000 symbol")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Public Inventory")]
        public bool IsPublic { get; set; }

        [Display(Name = "Inventory Image")]
        public IFormFile ImageFile { get; set; }

        public List<SelectListItem> CategoryOptions { get; set; }


    }
}

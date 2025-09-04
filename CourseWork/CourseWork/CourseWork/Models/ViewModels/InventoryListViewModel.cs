using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseWork.Models.ViewModels
{
    public class InventoryListViewModel
    {

        public List<Inventory> Inventories { get; set; } = new List<Inventory>();

        public string SearchQuery { get; set; }

        public int? SelectedCategoryId { get; set; }

        public int? SelectedTagId { get; set; } 

        public SelectList CategoryOptions { get; set; }
        public List<Tag> TagOptions { get; set; } = new List<Tag>();
    }
}

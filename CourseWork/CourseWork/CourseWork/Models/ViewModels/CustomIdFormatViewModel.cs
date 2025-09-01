using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models.ViewModels
{
    public class CustomIdFormatViewModel
    {
        public int Id { get; set; }
        public int inventoryId { get; set; }
        public List<CustomIdElementViewModel> Elements { get; set; } = new List<CustomIdElementViewModel>();
    }
}

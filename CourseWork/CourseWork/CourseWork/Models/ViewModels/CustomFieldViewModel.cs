using CourseWork.Models.Enums;

namespace CourseWork.Models.ViewModels
{
    public class CustomFieldViewModel
    {
        public int Id { get; set; }
        public List<CustomFieldElementViewModel> Elements { get; set; } = new List<CustomFieldElementViewModel>();
    }
}

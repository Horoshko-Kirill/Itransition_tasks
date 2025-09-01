using System.ComponentModel.DataAnnotations;
using CourseWork.Models.Enums;

namespace CourseWork.Models.ViewModels
{
    public class CustomIdElementViewModel
    {
        public int Id { get; set; }
        [Required]
        public CustomIdElementType Type { get; set; }
        public string? FixedValue { get; set; }
        public int Order { get; set; }
    }
}

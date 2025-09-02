using CourseWork.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models.ViewModels
{
    public class CustomFieldElementViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

       
        public string Description { get; set; }

        public CustomFieldType Type { get; set; }

        public bool ShowInTableView { get; set; }
        public int Order { get; set; }
    }
}

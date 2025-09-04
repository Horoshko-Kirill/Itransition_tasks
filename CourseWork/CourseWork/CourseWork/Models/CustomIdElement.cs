using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseWork.Models.Enums;

namespace CourseWork.Models
{
    public class CustomIdElement
    {

        public int Id { get; set; }

        public CustomIdElementType Type { get; set; }
        public int CustomIdFormatId { get; set; }

        public int Order { get; set; }

        public string? FixedValue { get; set; }

        [ForeignKey("CustomIdFormatId")]
        public CustomIdFormat CustomIdFormat { get; set; }

    }
}

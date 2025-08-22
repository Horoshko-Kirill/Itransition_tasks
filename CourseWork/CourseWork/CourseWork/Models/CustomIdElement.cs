using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CourseWork.Models.Enums;

namespace CourseWork.Models
{
    public class CustomIdElement
    {

        public int Id { get; set; }


        [Required]
        public CustomIdElementType Type { get; set; }

        [Required]
        public int CustomIdFormatId { get; set; }

        [ForeignKey("CustomIdFormatId")]
        public CustomIdFormat CustomIdFormat { get; set; }

    }
}

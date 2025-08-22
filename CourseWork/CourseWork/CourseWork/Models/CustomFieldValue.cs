using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class CustomFieldValue
    {

        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public Item Item { get; set; }

        [Required]
        public int CustomFieldId { get; set; }

        [ForeignKey("CustomFieldId")]
        public CustomField CustomField { get; set; }

        public string Value { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class CustomFieldValue
    {

        public int Id { get; set; }

        public int ItemId { get; set; }

        public Item? Item { get; set; }

        public int CustomFieldId { get; set; }

        [ForeignKey("CustomFieldId")]
        public CustomField? CustomField { get; set; }

        public string? Value { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using CourseWork.Models.Enums;

namespace CourseWork.Models
{
    public class CustomField
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 symbol")]
        public string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Description must not 2000 symbol")]
        public string Description { get; set; }

        [Required]
        public CustomFieldType FieldType { get; set; }

        public bool ShowInTableView { get; set; }

        public int DisplayOrder { get; set; }

        public int InventoryId { get; set; }

        public Inventory Inventory { get; set; }

        public ICollection<CustomFieldValue> CustomFields { get; set; } = new List<CustomFieldValue>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}

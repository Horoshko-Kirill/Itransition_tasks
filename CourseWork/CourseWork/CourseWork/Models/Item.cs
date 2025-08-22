using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class Item
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 symbol")]
        public string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Description must not 2000 symbol")]
        public string Description { get; set; }

        public string CustomId { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public Inventory Inventory { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CustomFieldValue> CustomFieldValues { get; set; } = new List<CustomFieldValue>();

    }
}

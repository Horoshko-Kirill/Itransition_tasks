using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class InventoryTag
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public Inventory Inventory { get; set; }

        [Required]
        public int TagId { get; set; }

        [ForeignKey("TagId")]
        public Tag Tag { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class CustomIdFormat
    {

        public int Id { get; set; }

        public string Description { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [ForeignKey("InventoryId")]
        public Inventory Inventory { get; set; }
        
        public ICollection<CustomIdElement> Elements { get; set; } = new List<CustomIdElement>();

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdateAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseWork.Models
{
    public class Inventory
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 symbol")]
        public string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Description must not 2000 symbol")]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ImageDropboxPath { get; set; }

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public bool isPublic { get; set; }

        [Required]
        public string CreatorId { get; set; }

        [ForeignKey("CreatorId")]
        public User Creator { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
        public CustomIdFormat CustomIdFormat { get; set; }
        public ICollection<Item> Items { get; set; }  = new List<Item>();
        public ICollection<CustomField> CustomFields { get; set; } = new List<CustomField>();
    }
}

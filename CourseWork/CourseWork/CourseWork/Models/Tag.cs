using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models
{
    public class Tag
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Tag name must not exceed 50 symbol")]
        public string Name { get; set; }

        public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();

        public DateTime CreatedDate { get; set; }

    }
}

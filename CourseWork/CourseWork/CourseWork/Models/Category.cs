using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models
{
    public class Category
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Category name must not exceed 50 symbol")]
        public string Name { get; set; }
        public string? DropboxPath { get; set; }
        public string? PhotoUrl { get; set; }
         
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    }
}

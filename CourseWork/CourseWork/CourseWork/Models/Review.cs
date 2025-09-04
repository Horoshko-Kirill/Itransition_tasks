using System.Runtime.CompilerServices;

namespace CourseWork.Models
{
    public class Review
    {

        public int Id { get; set; }

        public string Content { get; set; }

        public int Reating {  get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

    }
}

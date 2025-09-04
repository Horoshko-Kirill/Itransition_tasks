namespace CourseWork.Models
{
    public class InventoryLike
    {

        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public int inventoryId { get; set; }

        public Inventory Inventory { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}

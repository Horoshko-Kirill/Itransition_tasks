namespace CourseWork.Models
{
    public class Permission
    {

        public int Id { get; set; }

        public string UserId {  get; set; }
        public User User { get; set; }

        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public bool HaveWriteAccess { get; set; }
    }
}

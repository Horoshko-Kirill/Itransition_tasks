namespace CourseWork.Models.ViewModels
{
    public class MyInventoriesViewModel
    {

        public List<Inventory> CreatedInventories { get; set; }
        public List<Inventory> AccessibleInventories { get; set; }
        public List<Inventory> PublicInventories { get; set; }
        public string CurrentUserId { get; set; }
    }
}

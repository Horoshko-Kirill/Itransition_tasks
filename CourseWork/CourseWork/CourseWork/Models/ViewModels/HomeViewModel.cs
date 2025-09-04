namespace CourseWork.Models.ViewModels
{
    public class HomeViewModel
    {

        public List<InventoryWithLikesViewModel> PopularInventories { get; set; }
        public List<Inventory> LatestInventories { get; set; }

    }
}

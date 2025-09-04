namespace CourseWork.Models.ViewModels
{
    public class PermissionsViewModel
    {
        public int InventoryId { get; set; }
        public List<UserPermissionViewModel> Users { get; set; } = new List<UserPermissionViewModel>();
    }
}

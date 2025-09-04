namespace CourseWork.Models.ViewModels
{
    public class UserPermissionViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool HaveWriteAccess { get; set; }
    }
}

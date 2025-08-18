using Microsoft.AspNetCore.Identity;

namespace CourseWork.Models
{
    public class User : IdentityUser
    {

        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string? DropboxPath { get; set; }
        public string? PhotoUrl { get; set; }

        public ICollection<Inventory> OwnedInventories { get; set; }
        public ICollection<Permission> Permissions { get; set; }

    }
}

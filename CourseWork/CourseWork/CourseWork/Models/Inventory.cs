namespace CourseWork.Models
{
    public class Inventory
    {

        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<Permission> Permissions { get; set; }

    }
}

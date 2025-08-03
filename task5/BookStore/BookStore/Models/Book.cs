namespace BookStore.Models
{
    public class Book
    {

        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public string Publisher { get; set; }
        public int Likes { get; set; }
        public List<Review> Reviews { get; set; } = new();
        public string Picture { get; set; }

    }
}

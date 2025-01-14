namespace backend.Data.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Photo Photo { get; set; }
        public User User { get; set; }
    }
}

namespace backend.Data.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public Photo Photo { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public int PhotoId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

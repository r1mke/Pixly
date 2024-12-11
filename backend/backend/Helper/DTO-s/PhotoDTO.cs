using backend.Data.Models;

namespace backend.Helper.DTO_s
{
    public class PhotoDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LikeCount { get; set; }
        public int ViewCount { get; set; }
        public int Price { get; set; }
        public string Location { get; set; }
        public UserDTO? User { get; set; }
        public bool Approved { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Orientation { get; set; }
        public string Url { get; set; }
        public string? Size { get; set; }

        public bool IsLiked { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}

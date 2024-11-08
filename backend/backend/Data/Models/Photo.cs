using backend.Data.Models.Auth;

namespace backend.Data.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LikeCount { get; set; }
        public int ViewCount { get; set; }
        public int Price { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool Approved { get; set; }
        public DateTime CreateAt { get; set; }

        public ICollection<PhotoTag> PhotoTags { get; set; }
        public ICollection<PhotoResolution> Resolutions { get; set; }
    }
}

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
        public int? UserId { get; set; }
        public User? User { get; set; }
        public bool Approved { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Orientation { get; set; }
        public ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
        public ICollection<PhotoResolution> Resolutions { get; set; } = new List<PhotoResolution>();

        public ICollection<PhotoColor> PhotoColors { get; set; } = new List<PhotoColor>();
        public ICollection<PhotoCategory> PhotoCategories { get; set; } = new List<PhotoCategory>();
    }
}

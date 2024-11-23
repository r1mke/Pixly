namespace backend.Data.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }

        public ICollection<PhotoTag> PhotoTags { get; set; } = new List<PhotoTag>();
    }
}

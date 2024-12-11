namespace backend.Data.Models
{
    public class PhotoResolution
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public string Resolution { get; set; }
        public string Url { get; set; }
        public string? Size { get; set; }
        public DateTime? Date { get; set; }
    }
}

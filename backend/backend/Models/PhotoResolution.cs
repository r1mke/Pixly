namespace backend.Models
{
    public class PhotoResolution
    {
        public int Id { get; set; }
        public int PhotoId { get; set; } 
        public Photo Photo { get; set; }
        public string Resolution { get; set; }
        public string Url { get; set; }
        public long ? Size { get; set; }
        public DateTime? Date { get; set; }
    }
}

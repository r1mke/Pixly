namespace backend.Data.Models
{
    public class PhotoColor
    {
        public int PhotoId { get; set; }
        public int ColorId { get; set; }
        public Photo Photo { get; set; }
        public Color Color { get; set; }

       
    }
}

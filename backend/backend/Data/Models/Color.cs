namespace backend.Data.Models
{
    public class Color
    {
        public int Id {  get; set; }
        public string HexCode { get; set; } = string.Empty;
        public ICollection<PhotoColor> PhotoColors { get; set; } = new List<PhotoColor>();
    }
}

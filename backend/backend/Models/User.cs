namespace backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public ICollection<Photo> Photos { get; set; }
    }
}

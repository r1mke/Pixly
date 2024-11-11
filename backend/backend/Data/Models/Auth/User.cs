using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Data.Models.Auth
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public byte[]? ProfileImg { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
        public string RegisterCode { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsCreator { get; set; } = false;
        public ICollection<Photo> Photos { get; set; }
    }
}

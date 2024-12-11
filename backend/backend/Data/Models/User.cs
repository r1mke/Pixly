 using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? ProfileImgUrl { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }
        [JsonIgnore]
        public bool IsAdmin { get; set; }
        public bool IsCreator { get; set; }
        public ICollection<Photo> Photos { get; set; }
        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        
    }
}

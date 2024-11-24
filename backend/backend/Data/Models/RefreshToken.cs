using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Data.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt; // Token je istekao
        public bool IsRevoked => RevokedAt != null; // Token je opozvan
        public bool IsActive => !IsExpired && !IsRevoked; // Token je aktivan
    }
}

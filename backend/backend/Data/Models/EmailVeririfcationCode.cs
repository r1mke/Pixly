using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Data.Models
{
    public class EmailVerificationCode
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; } 
        public string ActivateCode { get; set; } 
        public DateTime SentAt { get; set; } 
        public DateTime ExpiryDate { get; set; } 
        public bool IsUsed { get; set; }
        public User User { get; set; }
    }
}

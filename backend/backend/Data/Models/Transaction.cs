using backend.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PhotoId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public decimal PlatformEarning { get; set; }
    public decimal AuthorEarning { get; set; }
    public string SessionId { get; internal set; }
    public string PaymentStatus { get; internal set; }
    public DateTime CreatedAt { get; internal set; }

    public User User { get; set; }
    public Photo Photo { get; set; }
}

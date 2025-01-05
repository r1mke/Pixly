using backend.Data.Models;

public class AuthorEarning
{
    public int Id { get; set; }
    public decimal TotalEarnings { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}

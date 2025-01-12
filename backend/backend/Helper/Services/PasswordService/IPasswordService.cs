namespace backend.Helper.Services.PasswordHasher
{
    public interface IPasswordService
    {
        Task<string> Hash(string password);
        Task<bool> Verify(string passwordHash, string inputPassword);
    }
}

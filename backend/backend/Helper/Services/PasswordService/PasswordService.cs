using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace backend.Helper.Services.PasswordHasher
{
    public class PasswordService : IPasswordService
    {
        public async Task<string> Hash(string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
            return passwordHash;
        }

        public async Task<bool> Verify(string passwordHash, string inputPassword)
        {
            var result = BCrypt.Net.BCrypt.EnhancedVerify(inputPassword, passwordHash);
            return result;
        }
    }
}

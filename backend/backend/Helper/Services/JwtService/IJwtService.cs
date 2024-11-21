using backend.Data.Models;

namespace backend.Helper.Services.JwtService
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
    }
}


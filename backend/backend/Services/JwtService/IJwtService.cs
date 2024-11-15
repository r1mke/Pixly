using backend.Data.Models;

namespace backend.Services.JwtService
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
    }
}


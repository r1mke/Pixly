using backend.Data.Models;
using System.Security.Claims;

namespace backend.Helper.Services.JwtService
{
    public interface IJwtService
    {
        string GenerateJwtToken(User user);
        RefreshToken GenerateRefreshToken(int userId);
        bool ValidateRefreshToken(RefreshToken refreshToken, string token);
        void RevokeRefreshToken(RefreshToken refreshToken);
        string ExtractEmailFromJwt(string jwtToken);
        public bool IsValidJwt(string token);
        void Logout(int userId);
    }
}

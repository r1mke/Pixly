using backend.Data;
using backend.Data.Models;
using Microsoft.AspNetCore.Mvc;
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
        Task<IActionResult> ValidateJwtAndUserAsync(string jwtToken, string refreshToken, AppDbContext db);
        public ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}

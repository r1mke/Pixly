using backend.Data;
using backend.Data.Models;
using backend.Helper.Services.JwtService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly AppDbContext db;

    public JwtService(IConfiguration configuration, AppDbContext db)
    {
        _secretKey = configuration["Jwt:SecretKey"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
        this.db = db;

        if (string.IsNullOrEmpty(_secretKey) || _secretKey.Length < 32)
            throw new InvalidOperationException("Secret key must be at least 256 bits long.");
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("Username", user.Username),
            new Claim("IsVerified", user.IsVerified.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(int userId)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };
    }

    public bool ValidateRefreshToken(RefreshToken refreshToken, string token)
    {
        if (refreshToken == null || !refreshToken.IsActive || refreshToken.Token != token)
            return false;
        
        return true;
    }

    public void RevokeRefreshToken(RefreshToken refreshToken)
    {
        if (refreshToken != null && refreshToken.IsActive)
            refreshToken.RevokedAt = DateTime.UtcNow;
        
    }

    public string? ExtractEmailFromJwt(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(jwtToken)) return null;

        var jwtTokenObj = handler.ReadJwtToken(jwtToken);

        var emailClaim = jwtTokenObj.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email);
        return emailClaim?.Value;
    }

    public bool IsValidJwt(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
                return false;

            var expiration = jsonToken.ValidTo; // UTC vrijeme isteka
            if (DateTime.UtcNow > expiration)
                return false;

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey))
            };

            handler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Logout(int userId)
    {
        // Nađi refresh token u bazi
        var refreshToken = db.RefreshTokens.FirstOrDefault(r => r.UserId == userId && r.RevokedAt == null);
        if (refreshToken != null)
        {
            // Poništi refresh token
            refreshToken.RevokedAt = DateTime.UtcNow;
            db.SaveChanges();
        }
    }

}

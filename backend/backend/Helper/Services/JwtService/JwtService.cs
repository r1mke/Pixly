using Azure;
using backend.Data;
using backend.Data.Models;
using backend.Helper.Services;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly AuthService authService;

    public JwtService(IConfiguration configuration, AuthService authService, AppDbContext db)
    {
        _secretKey = configuration["Jwt:SecretKey"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
        this.db = db;
        this.authService = authService;

        if (string.IsNullOrEmpty(_secretKey) || _secretKey.Length < 32)
            throw new InvalidOperationException("Secret key must be at least 256 bits long.");
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey))
        };

        try
        {
            var principal = handler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }


    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
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

            var expiration = jsonToken.ValidTo; // UTC time
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
        var refreshToken = db.RefreshTokens.FirstOrDefault(r => r.UserId == userId && r.RevokedAt == null);

        if (refreshToken != null)
        {
            refreshToken.RevokedAt = DateTime.UtcNow;
            db.SaveChanges();
        }
    }

    public async Task<IActionResult> ValidateJwtAndUserAsync(string jwtToken, string refreshToken, AppDbContext db)
    {
        if (!string.IsNullOrEmpty(jwtToken) && IsValidJwt(jwtToken))
        {
            var email = ExtractEmailFromJwt(jwtToken);

            if (string.IsNullOrEmpty(email))
                return new UnauthorizedObjectResult(new { IsValid = false, Message = "Invalid JWT token structure." });
        

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return new UnauthorizedObjectResult(new { IsValid = false, Message = "User not found." });
      
            return new OkObjectResult(user);
        }

        if (!string.IsNullOrEmpty(refreshToken))
        {
            var existingRefreshToken = await db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null);

            if (existingRefreshToken == null || !existingRefreshToken.IsActive || existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
   
                var jwtHandler = new JwtSecurityTokenHandler();
                if (jwtHandler.CanReadToken(jwtToken))
                {
                    var tokenObj = jwtHandler.ReadJwtToken(jwtToken);
                    var userEmail = tokenObj.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email)?.Value;
                
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (user != null)
                        Logout(user.Id);
                }

                return new UnauthorizedObjectResult(new { IsValid = false, Message = "Invalid or expired refresh token." });
            }

            var userFromRefreshToken = await db.Users.FirstOrDefaultAsync(u => u.Id == existingRefreshToken.UserId);
            if (userFromRefreshToken == null)
                return new UnauthorizedObjectResult(new { IsValid = false, Message = "User not found." });
        }

        return new UnauthorizedObjectResult(new { IsValid = false, Message = "Both tokens are invalid." });
    }
}

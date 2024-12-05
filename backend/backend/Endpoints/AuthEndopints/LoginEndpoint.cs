using Azure;
using backend.Data.Models;
using backend.Data;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LoginEndpoint;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Auth.PasswordHasher;
using backend.Heleper.Api;
using backend.Helper.Services;

[AllowAnonymous]
[Route("auth")]
public class LoginEndpoint(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService, AuthService authService) : MyEndpointBaseAsync
    .WithRequest<LoginUserRequest>
    .WithResult<UserLoginResponse>
{
    [HttpPost("login")]
    public override async Task<UserLoginResponse> HandleAsync([FromBody] LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        // check login user email
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), cancellationToken);
        if (user == null)
            return HandleError("Invalid email or password", 401);

        // check login user password
        var passwordValid = await passwordHasher.Verify(user.Password, request.Password);
        if (!passwordValid)
            return HandleError("Invalid email or password", 401);
        
        var existingRefreshToken = await db.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .FirstOrDefaultAsync(rt => rt.RevokedAt == null, cancellationToken);
     
        if (existingRefreshToken != null) // Revoke existingRefreshToken if exists
        {
            jwtService.RevokeRefreshToken(existingRefreshToken);
            db.RefreshTokens.Update(existingRefreshToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        // If User do not have refresh token, creating & saving in DB
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        db.RefreshTokens.Add(newRefreshToken);
        await db.SaveChangesAsync(cancellationToken);


        var jwtToken = jwtService.GenerateJwtToken(user);

        // Set Cookie
        authService.SetJwtCookie(jwtToken);
        authService.SetRefreshTokenCookie(newRefreshToken.Token);

        return new UserLoginResponse
        {
            JwtToken = jwtToken,
            RefreshToken = newRefreshToken.Token,
            Message = "Login successful",
            User = user
        };
    }

    private UserLoginResponse HandleError(string message, int statusCode)
    {
        Response.StatusCode = statusCode;
        return new UserLoginResponse { Message = message };
    }

    public class LoginUserRequest
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [MinLength(8), MaxLength(64)]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class UserLoginResponse
    {
        [JsonPropertyName("jwt")]
        public string JwtToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        public string Message { get; set; }
        public User User { get; set; }
    }
}

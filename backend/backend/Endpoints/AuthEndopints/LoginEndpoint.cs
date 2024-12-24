using Microsoft.AspNetCore.Mvc;
using backend.Data.Models;
using backend.Data;
using backend.Helper.Services.JwtService;
using backend.Helper.Auth.PasswordHasher;
using backend.Helper.Services;
using backend.Helper.Auth.EmailSender;
using Microsoft.EntityFrameworkCore;
using Azure;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Authorization;
using static LoginEndpoint;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

[AllowAnonymous]
[Route("auth")]
public class LoginEndpoint : MyEndpointBaseAsync
    .WithRequest<LoginUserRequest>
    .WithResult<UserLoginResponse>
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly AuthService _authService;
    private readonly IEmailSender _emailSender;
    private readonly IMemoryCache _memoryCache;


    public LoginEndpoint(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService, AuthService authService, IEmailSender emailSender,                      IMemoryCache memoryCache)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _authService = authService;
        _emailSender = emailSender;
        _memoryCache = memoryCache;
    }

    [HttpPost("login")]
    public override async Task<UserLoginResponse> HandleAsync([FromBody] LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user exists by email (case-insensitive)
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);
        if (user == null)
            return HandleError("Invalid email or password", 401);

        // Check if the password is valid
        var passwordValid = await _passwordHasher.Verify(user.Password, request.Password);
        if (!passwordValid)
            return HandleError("Invalid email or password", 401);


        HttpContext.Session.SetString("UserEmail", user.Email);

        // If TwoFactorEnabled, send 2FA code to the user's email
        if (user.TwoFactorEnabled)
        {
            var twoFaCode = new Random().Next(100000, 1000000).ToString();

            await _emailSender.SendEmailAsync(user.Email, "Verify your login", $"Code to verify your login: <strong>{twoFaCode}</strong>");
           
            HttpContext.Session.SetString("TwoFaCode", twoFaCode);

            return new UserLoginResponse
            {
                Message = "2FA code sent. Please verify.",
                StatusCode = 202,
            };
        }

        // If 2FA is not enabled, proceed to token generation
        return await GenerateTokensAsync(user, cancellationToken);
    }


    [HttpPost("verify-2fa")]
    public async Task<UserLoginResponse> Verify2Fa([FromBody] Verify2FaRequest request, CancellationToken cancellationToken)
    {
        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
            return HandleError("User session expired", 401);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail, cancellationToken);

        if (user == null)
            return HandleError("User not found", 404);

        var storedCode = HttpContext.Session.GetString("TwoFaCode");

        if (request.Code != storedCode)
            return HandleError("Invalid 2FA code", 401);

        // Once verified, generate JWT and refresh tokens
        return await GenerateTokensAsync(user, cancellationToken);
    }

    [HttpPost("resend-2fa")]
    public async Task<UserLoginResponse> Resend2FaCode(CancellationToken cancellationToken)
    {
        // Define the time window and request limit
        const int requestLimit = 1;
        const int timeWindowInSeconds = 120;

        var userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
            return HandleError("User session expired", 401);

   
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail, cancellationToken);

        if (user == null)
            return HandleError("User not found", 404);

        // Check if 2FA is enabled
        if (!user.TwoFactorEnabled)
            return HandleError("Two-factor authentication is not enabled for this user", 400);

        // Cache key for limiting requests
        var cacheKey = $"resend_2fa_{userEmail}";

        // Check the memory cache to see if the request count exceeds the limit
        if (_memoryCache.TryGetValue(cacheKey, out DateTime lastRequestTime))
            if (DateTime.UtcNow < lastRequestTime.AddSeconds(timeWindowInSeconds))
                return HandleError("Too many requests. Please try again later.", 400);
            
        
        // Generate new 2FA code
        var newTwoFaCode = new Random().Next(100000, 1000000).ToString();

        await _emailSender.SendEmailAsync(user.Email, "Verify your login", $"Code to verify your login: <strong>{newTwoFaCode}</strong>");

        HttpContext.Session.SetString("TwoFaCode", newTwoFaCode);

        // Update cache with the current time as the last request time
        _memoryCache.Set(cacheKey, DateTime.UtcNow, TimeSpan.FromSeconds(timeWindowInSeconds));

        return new UserLoginResponse
        {
            Message = "New 2FA code sent. Please verify.",
            StatusCode = 202
        };
    }



    private async Task<UserLoginResponse> GenerateTokensAsync(User user, CancellationToken cancellationToken)
    {
        var existingRefreshToken = await _db.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .FirstOrDefaultAsync(rt => rt.RevokedAt == null, cancellationToken);

        // Revoke the previous refresh token if any
        if (existingRefreshToken != null)
        {
            _jwtService.RevokeRefreshToken(existingRefreshToken);
            _db.RefreshTokens.Update(existingRefreshToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        // Generate and store a new refresh token
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync(cancellationToken);

        // Generate the JWT token
        var jwtToken = _jwtService.GenerateJwtToken(user);

        // Set the JWT and refresh token as cookies
        _authService.SetJwtCookie(jwtToken);
        _authService.SetRefreshTokenCookie(newRefreshToken.Token);

        return new UserLoginResponse
        {
            JwtToken = jwtToken,
            RefreshToken = newRefreshToken.Token,
            Message = "Login successful",
            User = user,
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

    public class Verify2FaRequest
    {
        [Required]
        [MinLength(6), MaxLength(6)]
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }

    public class UserLoginResponse
    {
        [JsonPropertyName("jwt")]
        public string JwtToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        public string Message { get; set; }
        public User User { get; set; }
        public int StatusCode { get; internal set; }
    }
}

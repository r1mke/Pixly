using Microsoft.AspNetCore.Mvc;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Data.Models;
using backend.Helper.Auth.PasswordHasher;
using backend.Helper.Auth.EmailSender;
using System.ComponentModel.DataAnnotations;
using static backend.Endpoints.AuthEndopints.RegisterEndpoint;
using Microsoft.AspNetCore.Authorization;
using backend.Helper.String;
using System.Text.Json.Serialization;
using backend.Helper.Services.JwtService;
using Azure;
using backend.Heleper.Api;
using System.Linq;
using backend.Helper.Services;

namespace backend.Endpoints.AuthEndopints
{
    [AllowAnonymous]
    [Route("auth")]
    public class RegisterEndpoint(AppDbContext db, IEmailSender emailSender, IPasswordHasher passwordHasher, IJwtService jwtService, IStringHelper stringHelper, AuthService authService) : MyEndpointBaseAsync
        .WithRequest<CreateUserRequest>
        .WithResult<UserRegistrationResponse>
    {
        [HttpPost("register")]
        public override async Task<UserRegistrationResponse> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            // checking Email
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            // checking Username
            var usernameExists = await db.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
            if (usernameExists)
                throw new InvalidOperationException("User with this username already exists");

            // saving User in DB
            var hash = await passwordHasher.Hash(request.Password);
            var user = new User
            {
                FirstName = stringHelper.CapitalizeFirstLetter(request.FirstName),
                LastName = stringHelper.CapitalizeFirstLetter(request.LastName),
                Username = request.Username,
                Email = request.Email.ToLower(),
                Password = hash,
                CreatedAt = DateTime.UtcNow,
                IsVerified = false,
                IsCreator = false,
                IsAdmin = false,
            };
            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            // creating & saving verification email code
            var verificationCode = new Random().Next(100000, 1000000).ToString();
            var newVerificationCode = new EmailVerificationCode
            {
                UserId = user.Id,
                ActivateCode = verificationCode,
                SentAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };
            db.EmailVerificationCodes.Add(newVerificationCode);
            await db.SaveChangesAsync(cancellationToken);

            // sending email code
            await emailSender.SendEmailAsync(user.Email, "Verify your email",
                $"Code to verify your email: <strong>{verificationCode}</strong>");

            // creating JWT and RefreshToken
            var jwtToken = jwtService.GenerateJwtToken(user);
            var refreshToken = jwtService.GenerateRefreshToken(user.Id);
            var refreshTokenRecord = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
                CreatedAt = DateTime.UtcNow
            };
            db.RefreshTokens.Add(refreshTokenRecord);
            await db.SaveChangesAsync(cancellationToken);

            // Set Cookie
            authService.SetJwtCookie(jwtToken);
            authService.SetRefreshTokenCookie(refreshToken.Token);

            return new UserRegistrationResponse
            {
                User = user,
                JwtToken = jwtToken,
                Message = "Registration successful"
            };
        }

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(string email, CancellationToken cancellationToken = default)
        {
            var emailExists = await db.Users.AnyAsync(u => u.Email == email, cancellationToken);
            var message = emailExists ? "Email is already taken!" : "Email is available";

            return Ok(new { available = !emailExists, message });
        }

        [HttpGet("check-username")]
        public async Task<IActionResult> CheckUsername(string username, CancellationToken cancellationToken = default)
        {
            var usernameExists = await db.Users.AnyAsync(u => u.Username == username, cancellationToken);
            var message = usernameExists ? "Username is already taken" : "Username is available";

            return Ok(new { available = !usernameExists, message });
        }

        public class CreateUserRequest
        {
            [Required]
            [JsonPropertyName("firstName")]
            public string FirstName { get; set; }

            [Required]
            [JsonPropertyName("lastName")]
            public string LastName { get; set; }

            [Required]
            [MinLength(5), MaxLength(20)]
            [JsonPropertyName("username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [Required]
            [MinLength(8), MaxLength(64)]
            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

        public class UserRegistrationResponse
        {
            public User User { get; set; }

            [JsonPropertyName("jwt")]
            public string JwtToken { get; set; }

            public string Message { get; set; }
        }
    }
}

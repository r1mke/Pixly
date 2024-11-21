using backend.Data.Models;
using backend.Data;
using backend.Heleper.Api;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static UserLoginEndpoint;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using backend.Helper.Auth.PasswordHasher;
using Microsoft.EntityFrameworkCore;

[AllowAnonymous]
[Route("auth")]
public class UserLoginEndpoint(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService) : MyEndpointBaseAsync
    .WithRequest<LoginUserRequest>
    .WithResult<UserLoginResponse>
{
    [HttpPost("login")]
    public override async Task<UserLoginResponse> HandleAsync([FromBody]LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return HandleError("Invalid login data", 400); // Pružanje korisničke poruke za loše podatke
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), cancellationToken);
        if (user == null)
        {
            return HandleError("Invalid email or password", 401); // Pružanje poruke za nepostojeći korisnik
        }

        var passwordValid = await passwordHasher.Verify(user.Password, request.Password);
        if (!passwordValid)
        {
            return HandleError("Invalid email or password", 401); // Pružanje poruke za lošu lozinku
        }

        var jwtToken = jwtService.GenerateJwtToken(user);

        return new UserLoginResponse
        {
            JwtToken = jwtToken,
            Message = "Login successful",
            User = user
        };
    }

    private UserLoginResponse HandleError(string message, int statusCode)
    {
        // Vraćanje odgovora sa statusnim kodom i porukom
        Response.StatusCode = statusCode;
        return new UserLoginResponse
        {
            Message = message
        };
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
        [JsonPropertyName("jwtToken")]
        public string JwtToken { get; set; }
        public string Message { get; set; }
        public User User { get; set; }
    }
}


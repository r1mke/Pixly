using Microsoft.AspNetCore.Mvc;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Auth.PasswordHasher;
using backend.Data.Models;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Authorization;
using static backend.Endpoints.AuthEndopints.UserLoginEndpoint;
using System.Text.Json.Serialization;
using backend.Helper.Services.JwtService;

namespace backend.Endpoints.AuthEndopints
{
    [AllowAnonymous]
    [Route("auth")]
    public class UserLoginEndpoint(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService) : MyEndpointBaseAsync
        .WithRequest<LoginUserRequest>
        .WithResult<UserLoginResponse>
    {
        [HttpPost("login")]
        public override async Task<UserLoginResponse> HandleAsync(LoginUserRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                throw new InvalidOperationException("Invalid login data");

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email.ToLower(), cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var passwordValid = await passwordHasher.Verify(user.Password, request.Password);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid email or password");


            var jwtToken = jwtService.GenerateJwtToken(user);

            return new UserLoginResponse
            {
                JwtToken = jwtToken,
                Message = "Login successful",
                User = user
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
}

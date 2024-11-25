using backend.Data;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.AuthEndpoints
{
    [Route("auth")]
    [ApiController]
    public class CurrentUserEndpoint(AppDbContext dbContext, IJwtService jwtService) : ControllerBase
    {
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var jwtToken = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(jwtToken))
                return Unauthorized(new { IsValid = false, Message = "Token is missing in cookies." });

            if (!jwtService.IsValidJwt(jwtToken))
                return Unauthorized(new { IsValid = false, Message = "Invalid or expired token." });

            var email = jwtService.ExtractEmailFromJwt(jwtToken);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { IsValid = false, Message = "Invalid token structure." });

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return Unauthorized(new { IsValid = false, Message = "User not found." });

            return Ok(new
            {
                IsValid = true,
                Message = "Token is valid.",
                User = new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    IsVerified = user.IsVerified,
                    IsAdmin = user.IsAdmin,
                }
            });
        }
    }
}

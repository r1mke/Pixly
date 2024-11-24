using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace backend.Endpoints.UserEndpoints
{
    [Route("auth")]
    public class CurrentUserEndpoint(AppDbContext dbContext) : ControllerBase
    {
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var jwtToken = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(jwtToken))
                return Unauthorized(new { Message = "JWT token not found in cookies." });

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var userId = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (userId == null)
                return Unauthorized(new { Message = "User is not authenticated." });

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
                return Unauthorized(new { Message = "User not found." });

            return Ok(new
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                IsVerified = user.IsVerified
            });
        }
    }
}

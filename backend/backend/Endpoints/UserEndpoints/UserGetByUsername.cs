using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.UserEndpoints
{
    [ApiController]
    public class UserGetByUsername(AppDbContext db) : ControllerBase
    {
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
            var user = await db.Users
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    UserId = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    ProfileImgUrl = u.ProfileImgUrl,
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(user);
        }
    }
}

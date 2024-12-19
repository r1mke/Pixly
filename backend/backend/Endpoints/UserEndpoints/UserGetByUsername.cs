using backend.Data;
using backend.Data.Models;
using backend.Helper.DTO_s;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.UserEndpoints
{
    [ApiController]
    public class UserGetByUsername(AppDbContext db, IJwtService jwtService) : ControllerBase
    {
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
            User? currentUser = null;

            // Opcionalna validacija JWT tokena
            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);
                if (validationResult is OkObjectResult okResult)
                {
                    currentUser = (User)okResult.Value;
                }
                else
                {
                    return Unauthorized(new { Message = "Invalid or expired token" });
                }
            }

           
            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .ThenInclude(p => p.Resolutions)
                .Where(u => u.Username == username)
                .Select(u => new 
                {
                    UserId = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    TotalViews = u.Photos.Sum(p => p.ViewCount),
                    TotalLikes = u.Photos.Sum(p => p.LikeCount),
                    ProfileImgUrl = u.ProfileImgUrl,
                    Photos = u.Photos.Select(p => new PhotoDTO
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        Price = p.Price,
                        Location = p.Location,
                        User = new UserDTO
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Username = u.Username,
                            Email = u.Email,
                            ProfileImgUrl = u.ProfileImgUrl
                        },
                        Approved = p.Approved,
                        CreateAt = p.CreateAt,
                        Orientation = p.Orientation,
                        Url = p.Resolutions
                            .Where(r => r.Resolution == "compressed")
                            .Select(r => r.Url)
                            .FirstOrDefault(),
                        Size = p.Resolutions
                            .Where(r => r.Resolution == "full_resolution")
                            .Select(r => r.Size)
                            .FirstOrDefault(),
                        LikeCount = p.LikeCount,
                        ViewCount = p.ViewCount,
                        IsLiked = currentUser != null && p.Likes.Any(l => l.UserId == currentUser.Id)
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(user);
        }
    }
}

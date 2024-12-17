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
            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);

            var userObj = validationResult is OkObjectResult okResult ? (User)okResult.Value : null;

            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Photos)
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
                            Id = p.User.Id,
                            FirstName = p.User.FirstName,
                            LastName = p.User.LastName,
                            Username = p.User.Username,
                            Email = p.User.Email,
                            ProfileImgUrl = p.User.ProfileImgUrl
                        },
                        Approved = p.Approved,
                        CreateAt = p.CreateAt,
                        Orientation = p.Orientation,
                        Url = db.PhotoResolutions
                            .Where(r => r.PhotoId == p.Id && r.Resolution == "compressed")
                            .Select(r => r.Url)
                            .FirstOrDefault(),
                        Size = db.PhotoResolutions
                            .Where(r => r.PhotoId == p.Id && r.Resolution == "full_resolution")
                            .Select(r => r.Size)
                            .FirstOrDefault(),
                        LikeCount = p.LikeCount,
                        ViewCount = p.ViewCount,
                        IsLiked = userObj!=null && db.Likes.Any(l => l.PhotoId == p.Id && l.User.Username == username)
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(user);
        }
    }
}

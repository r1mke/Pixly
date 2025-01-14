using backend.Data;
using backend.Data.Models;
using backend.Helper.DTO_s;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.UserEndpoints
{
    [ApiController]
    public class UserGetLikedPhotos(AppDbContext db, IJwtService jwtService) : ControllerBase
    {
        [HttpGet("user/{username}/liked-photos")]
        public async Task<IActionResult> GetLikedPhotoByUsernameAsync(string username)
        {
            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);

            var userObj = validationResult is OkObjectResult okResult ? (User)okResult.Value : null;

            var user = await db.Users
            .Include(u => u.Likes)
            .ThenInclude(l => l.Photo)
            .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return NotFound(new { Message = "User not found" });

            var likedPhotos = user.Likes.Where(l=>l.Photo.Approved==true).Select(l => new PhotoDTO
            {
                Id = l.Photo.Id,
                Title = l.Photo.Title,
                Description = l.Photo.Description,
                Price = l.Photo.Price,
                Location = l.Photo.Location,
                User = new UserDTO
                {
                    Id = l.Photo.User.Id,
                    FirstName = l.Photo.User.FirstName,
                    LastName = l.Photo.User.LastName,
                    Username = l.Photo.User.Username,
                    Email = l.Photo.User.Email,
                    ProfileImgUrl = l.Photo.User.ProfileImgUrl
                },
                Approved = l.Photo.Approved,
                CreateAt = l.Photo.CreateAt,
                Orientation = l.Photo.Orientation,
                Url = db.PhotoResolutions
                    .Where(r => r.PhotoId == l.Photo.Id && r.Resolution == "compressed")
                    .Select(r => r.Url)
                    .FirstOrDefault(),
                Size = db.PhotoResolutions
                    .Where(r => r.PhotoId == l.Photo.Id && r.Resolution == "full_resolution")
                    .Select(r => r.Size)
                    .FirstOrDefault(),
                LikeCount = l.Photo.LikeCount,
                ViewCount = l.Photo.ViewCount,
                IsLiked = userObj != null && db.Likes.Any(like => like.PhotoId == l.PhotoId  && like.User.Username == username),
                IsBookmarked = userObj != null && db.Bookmarks.Any(bookmark => bookmark.PhotoId == l.PhotoId && bookmark.User.Username == username),
            }).ToList();

            return Ok(likedPhotos);
        }
    }
}

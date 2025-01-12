using backend.Data;
using backend.Data.Models;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Helper.DTO_s;
using Newtonsoft.Json;

namespace backend.Endpoints.PhotoEndpoints
{
    [ApiController]
    [Route("api/photos/{id}")]
    public class PhotoGetById(AppDbContext db, IJwtService jwtService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PhotoDTO>> GetPhotoById(int id, CancellationToken cancellationToken = default)
        {
            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);
            var user = validationResult is OkObjectResult okResult ? (User)okResult.Value : null;

            var photo = await db.Photos
                .Where(p => p.Id == id)
                .Include(p => p.PhotoTags)
                    .ThenInclude(pt => pt.Tag)
                .Select(p => new PhotoDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    LikeCount = p.LikeCount,
                    ViewCount = p.ViewCount,
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
                        .Where(r => r.PhotoId == p.Id && r.Resolution == "compressed")
                        .Select(r => r.Size)
                        .FirstOrDefault(),
                    IsLiked = user != null && db.Likes.Any(l => l.PhotoId == p.Id && l.UserId == user.Id),
                    IsBookmarked = user != null && db.Bookmarks.Any(b => b.PhotoId == p.Id && b.UserId == user.Id),
                    Tags = p.PhotoTags.Select(pt => pt.Tag.TagName).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (photo == null)
            {
                return NotFound();
            }

            // Dohvati postojeći kolačić s pregledanim ID-jevima
            var viewedPhotosCookie = Request.Cookies["viewed_photos"];
            var viewedPhotos = new HashSet<int>();

            if (!string.IsNullOrEmpty(viewedPhotosCookie))
            {
                try
                {
                    viewedPhotos = JsonConvert.DeserializeObject<HashSet<int>>(viewedPhotosCookie);
                }
                catch (JsonException)
                {
                    viewedPhotos = new HashSet<int>();
                }
            }

            // Provjeri je li ID već u skupu
            if (!viewedPhotos.Contains(id))
            {
                var photoEntity = await db.Photos.FindAsync(id);
                if (photoEntity != null)
                {
                    photoEntity.ViewCount++;
                    await db.SaveChangesAsync(cancellationToken);

                    photo.ViewCount++;
                    viewedPhotos.Add(id);

                    // Ažuriraj kolačić s novim ID-jem
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddHours(24),
                        HttpOnly = true,
                        Secure = true
                    };

                    Response.Cookies.Append("viewed_photos", JsonConvert.SerializeObject(viewedPhotos), cookieOptions);
                }
            }

            // Dekodiraj tagove ako su u JSON formatu
            if (photo.Tags != null && photo.Tags.Count == 1 && photo.Tags[0].StartsWith("[") && photo.Tags[0].EndsWith("]"))
            {
                try
                {
                    photo.Tags = JsonConvert.DeserializeObject<List<string>>(photo.Tags[0]);
                }
                catch (JsonException)
                {
                    photo.Tags = new List<string>();
                }
            }

            return Ok(photo);
        }

    }
}

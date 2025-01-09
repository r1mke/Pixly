using backend.Data;
using backend.Data.Models;
using backend.Helper.DTO_s;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.PhotoEndpoints
{
    [ApiController]
    [Route("api/photos/get-purchased-photos")]
    public class PurchasedPhotos(AppDbContext db, IJwtService jwtService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetPurchasedPhotos(CancellationToken cancellationToken = default)
        {
            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            // Validacija JWT tokena i dohvatanje korisnika
            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);
            var user = validationResult is OkObjectResult okResult ? (User)okResult.Value : null;

            if (user == null)
            {
                return Unauthorized("User is not authorized.");
            }

            // Dohvatanje kupljenih fotografija
            var purchasedPhotos = await db.Transactions
                .Where(t => t.UserId == user.Id)
                .Select(t => new PhotoDTO
                {
                    Id = t.Photo.Id,
                    Title = t.Photo.Title,
                    Description = t.Photo.Description,
                    Price = t.Photo.Price,
                    Location = t.Photo.Location,
                    User = new UserDTO
                    {
                        Id = t.Photo.User.Id,
                        FirstName = t.Photo.User.FirstName,
                        LastName = t.Photo.User.LastName,
                        Username = t.Photo.User.Username,
                        Email = t.Photo.User.Email,
                        ProfileImgUrl = t.Photo.User.ProfileImgUrl
                    },
                    Approved = t.Photo.Approved,
                    CreateAt = t.Photo.CreateAt,
                    Orientation = t.Photo.Orientation,
                    Url = db.PhotoResolutions
                        .Where(r => r.Resolution == "compressed")
                        .Select(r => r.Url)
                        .FirstOrDefault(),
                    Size = db.PhotoResolutions
                        .Where(r => r.Resolution == "full_resolution")
                        .Select(r => r.Size)
                        .FirstOrDefault(),
                    LikeCount = t.Photo.LikeCount,
                    ViewCount = t.Photo.ViewCount,
                    IsLiked = db.Likes.Any(l => l.PhotoId == t.Photo.Id && l.UserId == user.Id),
                    Tags = t.Photo.PhotoTags.Select(pt => pt.Tag.TagName).ToList()
                })
                .ToListAsync(cancellationToken);

            // Provera da li postoje kupljene fotografije
            if (purchasedPhotos == null || !purchasedPhotos.Any())
            {
                return NotFound("No purchased photos found.");
            }

            return Ok(purchasedPhotos);
        }
    }
}

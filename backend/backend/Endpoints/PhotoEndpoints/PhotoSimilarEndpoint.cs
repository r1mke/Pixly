using backend.Data;
using backend.Data.Models;
using backend.Heleper.Api;
using backend.Helper.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using Sprache;
using System.Linq;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos/similar")]
    public class PhotoSimilarEndpoint(AppDbContext db) : MyEndpointBaseAsync.WithRequest<PhotoSimilarRequest>
        .WithResult<PhotoSimilarResult>
    {


        [HttpGet]
        public override async Task<PhotoSimilarResult> HandleAsync([FromQuery] PhotoSimilarRequest request, CancellationToken cancellationToken = default)
        {
            List<string> tags = request.Tags.Split(',').Select(tag => tag.Trim()).ToList();

            var photos = await db.Photos
               .Where(p => p.PhotoTags
                   .Any(pt => tags.Contains(pt.Tag.TagName.ToLower())))
               .Select(p => new PhotoDTO
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
                   Url = db.PhotoResolutions.Where(r => r.PhotoId == p.Id && r.Resolution == "compressed").Select(r => r.Url).FirstOrDefault(),
                   Size = db.PhotoResolutions.Where(r => r.PhotoId == p.Id && r.Resolution == "full_resolution").Select(r => r.Size).FirstOrDefault(),
                   // Provjera da li je korisnik lajkovao sliku
                   LikeCount = p.LikeCount,
                   ViewCount = p.ViewCount,
                   IsLiked = false
               })
            .ToArrayAsync(cancellationToken);

            return new PhotoSimilarResult
            {
                Photos = photos
            };

        }
    }

    public class PhotoSimilarResult
    {
        public PhotoDTO[] Photos { get; set; }
    }

    public class PhotoSimilarRequest
    {
        public string Tags { get; set; }
    }
}

using backend.Data;
using backend.Data.Models;
using backend.Heleper.Api;
using backend.Helper.DTO_s;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static backend.API.Endpoints.UserEndpoints.UserGetAllEndpoint;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos/page/{pageNumber}/{pageSize}")]
    public class PhotoGetAllEndpoint(AppDbContext db, IJwtService jwtService) : MyEndpointBaseAsync
        .WithRequest<PhotoGetAllRequest>
        .WithResult<PhotoGetAllResult>
    {
        
        [HttpGet]
        public override async Task<PhotoGetAllResult> HandleAsync([FromRoute] PhotoGetAllRequest request, CancellationToken cancellationToken = default)
        {
            if (request.pageNumber < 1 || request.pageSize < 1)
            {
                return new PhotoGetAllResult
                {
                    Photos = new PhotoGetAllDTO[0],
                    totalPhotos = 0,
                    totalPages = 0,
                    pageNumber = 0,
                    pageSize = 0,
                };
            }

            var skip = (request.pageNumber - 1) * request.pageSize;

            var totalPhotos = await db.Photos.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalPhotos / request.pageSize);

            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);

            var user = validationResult is OkObjectResult okResult ? (User)okResult.Value : null;

            var photos = await db.Photos
                .Select(p => new PhotoGetAllDTO
                {
                    Id = p.Id,
                    User = p.User,
                    Approved = p.Approved,
                    CreateAt = p.CreateAt,
                    LikeCount = p.LikeCount,
                    ViewCount = p.ViewCount,
                    Url = db.PhotoResolutions
                        .Where(r => r.PhotoId == p.Id && r.Resolution == "compressed")
                        .Select(r => r.Url)
                        .FirstOrDefault(),
                    IsLiked = user != null && db.Likes.Any(l => l.PhotoId == p.Id && l.UserId == user.Id)
                })
                .Skip(skip)
                .Take(request.pageSize)
                .ToArrayAsync(cancellationToken);

            return new PhotoGetAllResult
            {
                Photos = photos,
                totalPhotos = totalPhotos,
                totalPages = totalPages,
                pageNumber = request.pageNumber,
                pageSize = request.pageSize,
            };
        }

    }

    public class PhotoGetAllDTO
    {
        public int Id { get; set; }
        public User User { get; set; }
        public bool Approved { get; set; }
        public string? Url { get; set; }
        public bool IsLiked { get; set; }
        public int LikeCount { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreateAt { get; set; }

    }

    public class PhotoGetAllResult
    {
        public PhotoGetAllDTO[] Photos { get; set; }
        public int totalPhotos { get; set; }
        public int totalPages { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }


    public class PhotoGetAllRequest
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }

}

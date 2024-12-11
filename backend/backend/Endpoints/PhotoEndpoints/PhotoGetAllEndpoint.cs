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
    [Route("api/photos")]
    public class PhotoGetAllEndpoint(AppDbContext db, IJwtService jwtService) : MyEndpointBaseAsync
        .WithRequest<PhotoGetAllRequest>
        .WithResult<PhotoGetAllResult>
    {
        

       
        [HttpGet]
        public override async Task<PhotoGetAllResult> HandleAsync([FromQuery] PhotoGetAllRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1 || request.PageSize < 1)
            {
                return new PhotoGetAllResult
                {
                    Photos = new PhotoGetAllDTO[0],
                    TotalPhotos = 0,
                    TotalPages = 0,
                    PageNumber = 0,
                    PageSize = 0,
                };
            }

            var skip = (request.PageNumber - 1) * request.PageSize;

            var totalPhotos = await db.Photos.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalPhotos / request.PageSize);

            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);

            var user = validationResult is OkObjectResult okResult ? (User)okResult.Value : null;

            var photos = await db.Photos
                .Select(p => new PhotoGetAllDTO
                {
                    Id = p.Id,
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
                    Url = db.PhotoResolutions.Where(r => r.PhotoId == p.Id && r.Resolution == "compressed").Select(r => r.Url).FirstOrDefault(),
                    // Provjera da li je korisnik lajkovao sliku
                    LikeCount = p.LikeCount,
                    ViewCount = p.ViewCount,
                    
                    IsLiked = user != null && db.Likes.Any(l => l.PhotoId == p.Id && l.UserId == user.Id)
                })
                .Skip(skip)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new PhotoGetAllResult
            {
                Photos = photos,
                TotalPhotos = totalPhotos,
                TotalPages = totalPages,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }

    }

    public class PhotoGetAllDTO
    {
        public int Id { get; set; }
        public UserDTO User { get; set; }
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
        public int TotalPhotos { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class PhotoGetAllRequest : PaginationRequest
    {
        
    }

}

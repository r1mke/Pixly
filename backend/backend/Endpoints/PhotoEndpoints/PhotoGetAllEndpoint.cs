using backend.Data;
using backend.Data.Models;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static backend.API.Endpoints.UserEndpoints.UserGetAllEndpoint;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos")]
    public class PhotoGetAllEndpoint : MyEndpointBaseAsync
        .WithoutRequest.WithResult<PhotoGetAllResult[]>
    {
            private AppDbContext _db;
            public PhotoGetAllEndpoint(AppDbContext db)
            {
                _db = db;
            }

        [HttpGet]
        public override async Task<PhotoGetAllResult[]> HandleAsync(CancellationToken cancellationToken = default)
        {
            var photos = await _db.Photos
                .AsNoTracking()
                .Select(p => new PhotoGetAllResult
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    LikeCount = p.LikeCount,
                    ViewCount = p.ViewCount,
                    Price = p.Price,
                    Location = p.Location,
                    User = p.User,
                    Approved = p.Approved,
                    CreateAt = p.CreateAt,
                    Orientation = p.Orientation,
                    Url = _db.PhotoResolutions.Where(r => r.PhotoId == p.Id && r.Resolution == "compressed").Select(r=>r.Url).FirstOrDefault(),
                    Size = _db.PhotoResolutions.Where(r => r.PhotoId == p.Id && r.Resolution == "compressed").Select(r => r.Size).FirstOrDefault(),

                })
                .ToArrayAsync(cancellationToken);



            return photos;
        }
    }

    public class PhotoGetAllResult
    { 
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LikeCount { get; set; }
        public int ViewCount { get; set; }
        public int Price { get; set; }
        public string Location { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public bool Approved { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Orientation { get; set; }
        public string Url { get; set; }
        public long? Size { get; set; }

    }
}

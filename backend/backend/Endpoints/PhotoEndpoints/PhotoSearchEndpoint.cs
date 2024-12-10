using backend.Data;
using backend.Heleper.Api;
using backend.Helper.DTO_s;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos/search")]
    public class PhotoSearchEndpoint : MyEndpointBaseAsync.WithRequest<SearchRequest>.WithResult<SearchResult>
    {
        private readonly AppDbContext _db;

        public PhotoSearchEndpoint(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public override async Task<SearchResult> HandleAsync([FromQuery] SearchRequest request, CancellationToken cancellationToken = default)
        {

            string? title = request.Title;

            var query = _db.Photos
             .Include(p => p.Resolutions)
             .Include(p => p.PhotoTags)
                 .ThenInclude(pt => pt.Tag)
             .Where(p =>
                 (title == null ||
                  p.Title.Contains(title) ||
                  p.Description.Contains(title) ||
                  p.PhotoTags.Any(pt => pt.Tag.TagName.Contains(title))) &&
                 (string.IsNullOrEmpty(request.Size) ||
                  _db.PhotoResolutions.Any(pr =>
                      pr.Photo.Id == p.Id && pr.Size.ToString() == request.Size)) &&
                 (string.IsNullOrEmpty(request.Orientation) ||
                  p.Orientation == request.Orientation)
             )
             .Select(p => new PhotoDTO
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
                 Url = _db.PhotoResolutions
                     .Where(pr => pr.Photo.Id == p.Id && pr.Resolution == "compressed")
                     .Select(pr => pr.Url)
                     .FirstOrDefault(),
                 Size = _db.PhotoResolutions
                     .Where(pr => pr.Photo.Id == p.Id && pr.Resolution == "compressed")
                     .Select(pr => pr.Size)
                     .FirstOrDefault()
             });


            var photos = await query.ToArrayAsync(cancellationToken);

            return new SearchResult
            {
                photos = photos
            };
        }
    }

    public class SearchRequest
    {
        public string? Title { get; set; }
        public string? Size { get; set; }
        public string? Orientation { get; set; }
    }

    public class SearchResult
    {
        public PhotoDTO[]? photos { get; set; }
    }
}
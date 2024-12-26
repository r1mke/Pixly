using backend.Data;
using backend.Data.Models;
using backend.Heleper.Api;
using backend.Helper.DTO_s;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos/search")]
    public class PhotoSearchEndpoint(IJwtService jwtService, AppDbContext _db) : MyEndpointBaseAsync.WithRequest<SearchRequest>.WithResult<SearchResult>
    {
        [HttpGet]
        public override async Task<SearchResult> HandleAsync( [FromQuery] SearchRequest request, CancellationToken cancellationToken = default)
        {

            string? title = request.Title;
            string? popularity = request.Popularity;
            string ? size = request.Size == "All Sizes" ? null : request.Size;
            string? orientation = request.Orientation == "All Orientations" ? null : request.Orientation;
            string? color = string.IsNullOrEmpty(request.Color) ? null : request.Color;

            if (request.PageNumber < 1 || request.PageSize < 1)
            {
                return new SearchResult
                {
                    Photos = new PhotoDTO[0],
                    TotalPhotos = 0,
                    TotalPages = 0,
                    PageNumber = 0,
                    PageSize = 0,
                };
            }

            var skip = (request.PageNumber - 1) * request.PageSize;

            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];
            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, _db);

            var user = validationResult is OkObjectResult okResult ? (User)okResult.Value : null;

            var query = _db.Photos
             .Include(p => p.Resolutions)
             .Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag)
             .Include(p => p.PhotoColors).ThenInclude(pc => pc.Color)
             .Where(p =>
                 (title == null ||
                  p.Title.Contains(title) ||
                  p.Description.Contains(title) ||
                  p.PhotoTags.Any(pt => pt.Tag.TagName.Contains(title))) &&
                (p.Approved == true) &&
                 (orientation == null || p.Orientation == orientation.ToLower()) &&
                 (size == null || p.Resolutions.Any(pr => pr.Size.ToLower() == size.ToLower() && pr.Resolution == "full_resolution")) &&
                 (color == null || p.PhotoColors.Any(pc => pc.Color.HexCode.ToLower() == color.ToLower())))
             .Select(p => new
             {
                 Photo = p,
                 CompressedResolution = p.Resolutions.FirstOrDefault(pr => pr.Resolution == "compressed")
             });

            var photos = await query.Select(result => new PhotoDTO
            {
                Id = result.Photo.Id,
                Title = result.Photo.Title,
                Description = result.Photo.Description,
                LikeCount = result.Photo.LikeCount,
                ViewCount = result.Photo.ViewCount,
                Price = result.Photo.Price,
                Location = result.Photo.Location,
                User = new UserDTO
                {
                    Id = result.Photo.User.Id,
                    FirstName = result.Photo.User.FirstName,
                    LastName = result.Photo.User.LastName,
                    Username = result.Photo.User.Username,
                    Email = result.Photo.User.Email,
                    ProfileImgUrl = result.Photo.User.ProfileImgUrl
                },
                Approved = result.Photo.Approved,
                CreateAt = result.Photo.CreateAt,
                Orientation = result.Photo.Orientation,
                Url = result.CompressedResolution.Url,
                Size = result.CompressedResolution.Size,
                IsLiked = request.UserId == null ? false : _db.Likes.Any(l => l.Photo.Id == result.Photo.Id && l.UserId == request.UserId)

            }).Skip(skip)
              .Take(request.PageSize)
              .ToArrayAsync();

            var totalPhotos =  photos.Count();
            var totalPages = (int)Math.Ceiling((double)totalPhotos / request.PageSize);

            PhotoDTO[] photosResult = photos; // Defaultni set podataka

            if (!string.IsNullOrEmpty(popularity))
            {
                if (popularity.ToLower() == "trending")
                    photosResult = photos.OrderByDescending(p => p.LikeCount).ToArray();
                else if (popularity.ToLower() == "latest")
                    photosResult = photos.OrderByDescending(p => p.CreateAt).ToArray();
            }
            Console.WriteLine(photos[0].IsLiked);

            return new SearchResult
            {
                Photos = photosResult == null ? Array.Empty<PhotoDTO>() : photosResult,
                TotalPhotos = totalPhotos,
                TotalPages = totalPages,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };


        }
    }

    public class SearchRequest : PaginationRequest
    {
        public string Popularity { get; set; }
        public string Title { get; set; }
        public string? Orientation { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int? UserId { get; set; }
    }

    public class SearchResult
    {
        public PhotoDTO[]? Photos {  get; set; }
        public int TotalPhotos { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
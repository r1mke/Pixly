using backend.Data;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.CategoryEndpoint
{
    [Route("api/categories")]
    public class CategoryGetAllEndpoint : MyEndpointBaseAsync.WithoutRequest.WithResult<CategoryGetAllResult>
    {
        private readonly AppDbContext _db;

        public CategoryGetAllEndpoint(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public override async Task<CategoryGetAllResult> HandleAsync(CancellationToken cancellationToken = default)
        {
            var result = from category in _db.Categories
                         join photocategories in _db.PhotoCategories on category.Id equals photocategories.CategoryId
                         join photo in _db.Photos on photocategories.PhotoId equals photo.Id
                         join resolution in _db.PhotoResolutions on photo.Id equals resolution.PhotoId
                         where resolution.Resolution.Contains("compressed")
                         group new { category, photo, resolution } by new { category.Id, category.Name } into g
                         select new
                         {
                             g.Key.Id,
                             g.Key.Name,
                             Photos = g.ToList()
                         };

            // Now execute the query and bring it into memory
            var categories = await result.ToListAsync(cancellationToken);

            // Process the grouped data in memory to get the most liked photo per category
            var categoryResults = categories.Select(g => new CategoryEndpoint
            {
                Id = g.Id,
                Name = g.Name,
                CategoryPhotoUrl = g.Photos.OrderByDescending(x => x.photo.LikeCount).FirstOrDefault()?.resolution.Url,
                NumberOfPhotos = g.Photos.Count
            }).ToList();

            return new CategoryGetAllResult
            {
                Categories = categoryResults
            };
        }
    }

    public class CategoryGetAllResult
    {
        public List<CategoryEndpoint> Categories { get; set; } = new List<CategoryEndpoint>();
    }

    public class CategoryEndpoint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryPhotoUrl { get; set; }
        public int NumberOfPhotos { get; set; }
    }
}

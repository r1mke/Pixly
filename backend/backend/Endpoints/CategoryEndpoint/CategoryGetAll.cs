using backend.Data;
using backend.Heleper.Api;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.CategoryEndpoint
{
    [Route("api/categories/all")]
    public class CategoryGetAll : MyEndpointBaseAsync.WithoutRequest.WithResult<AllCategories[]>
    {
        private readonly AppDbContext _db;
        public CategoryGetAll(AppDbContext db) {
        _db = db;
        }

        [HttpGet]
        public override async Task<AllCategories[]> HandleAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _db.Categories
                 .AsNoTracking()
                 .Select(c => new AllCategories
                 {
                     Id = c.Id,
                     CategoryName = c.Name
                 }).ToArrayAsync(cancellationToken);

            return categories;
        }
    }

    public class AllCategories
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }
}

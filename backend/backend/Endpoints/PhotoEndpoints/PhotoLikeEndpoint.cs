using backend.Data;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Mvc;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photo/:id/like")]
    public class PhotoLikeEndpoint : MyEndpointBaseAsync.WithRequest<PhotoLikeRequest>.WithActionResult<int>
    {
        private readonly AppDbContext _db;

        public PhotoLikeEndpoint(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public override Task<ActionResult<int>> HandleAsync(PhotoLikeRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    public class PhotoLikeRequest
    {
       public string jwt { get; set; }
       public int photoId { get; set; }
    }
}

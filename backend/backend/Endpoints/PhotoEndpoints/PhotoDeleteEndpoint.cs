using backend.Data;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos")]
    public class PhotoDeleteEndpoint : MyEndpointBaseAsync
        .WithRequest<int>.WithActionResult
    {
        private AppDbContext _db;

        public PhotoDeleteEndpoint(AppDbContext db)
        {
            _db = db;
        }
        [HttpDelete("{id:int}")]
        public override async Task<ActionResult> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var photo = await _db.Photos.FirstOrDefaultAsync(p=> p.Id == id, cancellationToken);

            if (photo == null)
            {
                return NotFound($"Photo with ID: {id} not found");
            }

            _db.Photos.Remove(photo);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

using backend.Data;
using backend.Helper.Services.PhotoService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("photos")]
    [ApiController]
    public class PhotoBookmarkController(AppDbContext db, IPhotoService photoService) : ControllerBase
    {
        [HttpPost("{photoId}/bookmark")]
        public async Task<IActionResult> BookmarkPhoto(int photoId, [FromQuery] int userId, CancellationToken cancellationToken)
        {
            var photo = await photoService.GetPhotoByIdAsync(photoId, cancellationToken);
            if (photo == null)
                return NotFound(new { Message = "Photo not found" });

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var existingBookmark = await photoService.GetBookmarkAsync(photoId, userId, cancellationToken = default);
            if (existingBookmark != null)
                return BadRequest(new { Message = "You have already bookmarked this photo" });

            await photoService.BookmarkPhotoAsync(photoId, userId, cancellationToken);

            return Ok(new { Message = "Photo bookmarked successfully" });
        }

        [HttpDelete("{photoId}/bookmark")]
        public async Task<IActionResult> UnBookmarkPhoto(int photoId, [FromQuery] int userId, CancellationToken cancellationToken)
        {
            var photo = await photoService.GetPhotoByIdAsync(photoId, cancellationToken);
            if (photo == null)
                return NotFound(new { Message = "Photo not found" });

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var existingBookmark = await photoService.GetBookmarkAsync(photoId, userId);
            if (existingBookmark == null)
                return BadRequest(new { Message = "You have not bookmarked this photo" });

            await photoService.UnBookmarkPhotoAsync(photoId, userId, cancellationToken);

            return Ok(new { Message = "Photo unbookmarked successfully" });
        }
    }
}

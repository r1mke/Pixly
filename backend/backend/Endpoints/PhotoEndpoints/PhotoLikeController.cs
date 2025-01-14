using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Models;
using backend.Helper.Services.PhotoService;

namespace backend.Controllers
{
    [Route("api/photos")]
    [ApiController]
    public class PhotoLikeController(AppDbContext db, IPhotoService photoService) : ControllerBase
    {
 
        [HttpPost("{photoId}/like")]
        public async Task<IActionResult> LikePhoto(int photoId, [FromQuery] int userId, CancellationToken cancellationToken)
        {
            var photo = await photoService.GetPhotoByIdAsync(photoId, cancellationToken);
            if (photo == null)
                return NotFound(new { Message = "Photo not found" });

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var existingLike = await photoService.GetLikeAsync(photoId, userId, cancellationToken);
            if (existingLike != null)
                return BadRequest(new { Message = "You have already liked this photo" });

            await photoService.LikePhotoAsync(photoId, userId, cancellationToken);

            return Ok(new LikeUnlikeResponse
            {
                Message = "Photo liked successfully",
                LikeCount = photo.LikeCount
            });
        }


        [HttpDelete("{photoId}/like")]
        public async Task<IActionResult> UnlikePhoto(int photoId, [FromQuery] int userId, CancellationToken cancellationToken)
        {
            var photo = await photoService.GetPhotoByIdAsync(photoId, cancellationToken);
            if (photo == null)
                return NotFound(new { Message = "Photo not found" });

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var like = await photoService.GetLikeAsync(photoId, userId, cancellationToken);
            if (like == null)
                return BadRequest(new { Message = "You have not liked this photo yet" });

            await photoService.UnlikePhotoAsync(photoId, userId, cancellationToken);

            return Ok(new LikeUnlikeResponse
            {
                Message = "Photo unliked successfully",
                LikeCount = photo.LikeCount
            });
        }

        public class LikeUnlikeResponse
        {
            public string Message { get; set; }
            public int LikeCount { get; set; }
        }

    }
}

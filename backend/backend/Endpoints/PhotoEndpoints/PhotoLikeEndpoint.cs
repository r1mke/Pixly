using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Models;

namespace backend.Controllers
{
    [Route("api/photos")]
    [ApiController]
    public class PhotoLikeController(AppDbContext db) : ControllerBase
    {
 
        [HttpPost("{photoId}/like")]
        public async Task<IActionResult> LikePhoto(int photoId, [FromQuery] int userId, CancellationToken cancellationToken)
        {
            var photo = await db.Photos.FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken);
            if (photo == null)
                return NotFound(new { Message = "Photo not found" });

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var existingLike = await db.Likes.FirstOrDefaultAsync(l => l.PhotoId == photoId && l.UserId == userId, cancellationToken);
            if (existingLike != null)
                return BadRequest(new { Message = "You have already liked this photo" });

            var like = new Like
            {
                PhotoId = photoId,
                UserId = userId
            };

            db.Likes.Add(like);
            photo.LikeCount++;
            await db.SaveChangesAsync(cancellationToken);

            return Ok(new LikeUnlikeResponse
            {
                Message = "Photo liked successfully",
                LikeCount = photo.LikeCount
            });
        }


        [HttpDelete("{photoId}/like")]
        public async Task<IActionResult> UnlikePhoto(int photoId, [FromQuery] int userId, CancellationToken cancellationToken)
        {
            var photo = await db.Photos.FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken);
            if (photo == null)
                return NotFound(new { Message = "Photo not found" });

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var like = await db.Likes.FirstOrDefaultAsync(l => l.PhotoId == photoId && l.UserId == userId, cancellationToken);
            if (like == null)
                return BadRequest(new { Message = "You have not liked this photo yet" });

            db.Likes.Remove(like);
            photo.LikeCount--;
            await db.SaveChangesAsync(cancellationToken);

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

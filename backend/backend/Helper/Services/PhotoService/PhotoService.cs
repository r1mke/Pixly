using backend.Data;
using backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Helper.Services.PhotoService
{
    public class PhotoService(AppDbContext db) : IPhotoService
    {
        public async Task<Photo> GetPhotoByIdAsync(int photoId, CancellationToken cancellationToken = default)
        {
            return await db.Photos.FindAsync(photoId, cancellationToken);
        }

        public async Task<Like> GetLikeAsync(int photoId, int userId, CancellationToken cancellationToken = default)
        {
            return await db.Likes.FirstOrDefaultAsync(l => l.PhotoId == photoId && l.UserId == userId, cancellationToken);
        }

        public async Task LikePhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default)
        {
            var like = new Like
            {
                PhotoId = photoId,
                UserId = userId
            };

            db.Likes.Add(like);
            var photo = await GetPhotoByIdAsync(photoId, cancellationToken);
            photo.LikeCount++;
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task UnlikePhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default)
        {
            var like = await GetLikeAsync(photoId, userId, cancellationToken);
            db.Likes.Remove(like);
            var photo = await GetPhotoByIdAsync(photoId, cancellationToken);
            photo.LikeCount--;
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task BookmarkPhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default)
        {
            var bookmark = new Bookmark
            {
                PhotoId = photoId,
                UserId = userId
            };

            db.Bookmarks.Add(bookmark);
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task UnBookmarkPhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default)
        {
            var bookmark = await db.Bookmarks.FirstOrDefaultAsync(b => b.PhotoId == photoId && b.UserId == userId, cancellationToken);
            db.Bookmarks.Remove(bookmark);
            await db.SaveChangesAsync(cancellationToken);
        }

        public async Task<Bookmark> GetBookmarkAsync(int photoId, int userId, CancellationToken cancellationToken = default)
        {
            return await db.Bookmarks.FirstOrDefaultAsync(b => b.PhotoId == photoId && b.UserId == userId, cancellationToken);
        }
    }
}

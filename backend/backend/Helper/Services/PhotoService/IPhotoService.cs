using backend.Data.Models;

namespace backend.Helper.Services.PhotoService
{
    public interface IPhotoService
    {
        Task<Photo> GetPhotoByIdAsync(int photoId, CancellationToken cancellationToken = default);
        Task<Like> GetLikeAsync(int photoId, int userId, CancellationToken cancellationToken = default);
        Task<Bookmark> GetBookmarkAsync(int photoId, int userId, CancellationToken cancellationToken = default);
        Task LikePhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default);
        Task UnlikePhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default);
        Task BookmarkPhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default);
        Task UnBookmarkPhotoAsync(int photoId, int userId, CancellationToken cancellationToken = default);
    }
}

using backend.Data;
using backend.Heleper.Api;
using backend.Helper.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.Admin
{
    [Route("api/admin/photos")]
    public class GetAllPhotos : MyEndpointBaseAsync.WithoutRequest.WithResult<GetAllPhotosResult>
    {
        private readonly AppDbContext _db;
        public GetAllPhotos(AppDbContext db) { _db = db; }

        [HttpGet]
        public override async Task<GetAllPhotosResult> HandleAsync(CancellationToken cancellationToken = default)
        {
            var query = _db.Photos
             .Include(p => p.Resolutions)
             .Include(p => p.PhotoTags).ThenInclude(pt => pt.Tag)
             .Where(p=>p.Approved == false)
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
                Url = result.CompressedResolution.Url
            }).ToArrayAsync();

            return new GetAllPhotosResult
            {
                Photos = photos,
            };
        }
    }

    public class GetAllPhotosResult
    {
        public PhotoDTO[]? Photos { get; set; }
    }
}

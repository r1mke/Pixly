using backend.Data;
using backend.Data.Models;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Mvc;

namespace backend.Endpoints.PhotoEndpoints
{
    [ApiController]
    [Route("api/photoApproved")]
    public class PhotoApprovedEndpoint : MyEndpointBaseAsync.WithRequest<PhotoApprovedRequest>.WithResult<PhotoApprovedResult>
    {
        private readonly PhotoCloudinaryService _photoService;
        private readonly AppDbContext _db;

        public PhotoApprovedEndpoint(PhotoCloudinaryService photoService, AppDbContext db)
        {
            _photoService = photoService;
            _db = db;
        }

        [HttpPut]
        public override async Task<PhotoApprovedResult> HandleAsync(PhotoApprovedRequest request, CancellationToken cancellationToken = default)
        {
            var updatedPhoto = await _photoService.ApprovedPhotoAsync(request.PhotoId, request.Approved);

            if (updatedPhoto == null)
            {
                return new PhotoApprovedResult
                {
                    Message = "Photo not found or not owned by the user.",
                    Success = false
                };
            }

            return new PhotoApprovedResult
            {
                Message = "Photo approved successfully.",
                Success = true,
                PhotoId = updatedPhoto.Id
            };
        }
    }


    public class PhotoApprovedRequest
    { 
        public int PhotoId { get; set; } 
        public bool Approved { get; set; }
    }

    public class PhotoApprovedResult
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int? PhotoId { get; set; }
    }

}

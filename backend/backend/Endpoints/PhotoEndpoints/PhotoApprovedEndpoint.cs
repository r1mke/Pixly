using backend.Data;
using Microsoft.AspNetCore.Mvc;

namespace backend.Endpoints.PhotoEndpoints
{
    [ApiController]
    [Route("api/photoApproved")]
    public class PhotoApprovedEndpoint : ControllerBase
    {
        private readonly PhotoService _photoService;
        private readonly AppDbContext _db;

        public PhotoApprovedEndpoint(PhotoService photoService, AppDbContext db)
        {
            _photoService = photoService;
            _db = db;
        }

        [HttpPut("{photoId}")]
        public async Task<IActionResult> ApprovedPhoto([FromRoute] int photoId, [FromForm] PhotoApprovedRequest request)
        {
            try
            {
                var updatedPhoto = await _photoService.ApprovedPhotoAsync(photoId, request.Approved);

                if (updatedPhoto == null)
                {
                    return NotFound(new PhotoApprovedResult
                    {
                        Message = "Photo not found or not owned by the user.",
                        Success = false
                    });
                }

                return Ok(new PhotoApprovedResult
                {
                    Message = "Photo approved successfully.",
                    Success = true,
                    PhotoId = updatedPhoto.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new PhotoApprovedResult
                {
                    Message = $"Error: {ex.Message}",
                    Success = false
                });
            }
        }
    }


    public class PhotoApprovedRequest
    { 
        public bool Approved { get; set; }
    }

    public class PhotoApprovedResult
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int? PhotoId { get; set; }
    }

}

using backend.Data.Models;
using backend.Helper.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos")]
    [ApiController]
    public class PhotoUpdateEndpoint(PhotoService photoService) : ControllerBase
    {
        [HttpPut("{photoId}")]
        public async Task<IActionResult> UpdatePhoto([FromRoute] int photoId, [FromForm] UpdatePhotoRequest request)
        {
            if (string.IsNullOrEmpty(request.Description) && string.IsNullOrEmpty(request.Title) && string.IsNullOrEmpty(request.Location))
            {
                return BadRequest(new UpdatePhotoResult
                {
                    Message = "At least one field (Description, Tags, Location, or Price) must be provided to update.",
                    Success = false
                });
            }

            try
            {
                var updatedPhoto = await photoService.UpdatePhotoAsync(
                    photoId,
                    request.Title,
                    request.Description,
                    request.Location
                    //request.Price
                );

                if (updatedPhoto == null)
                {
                    return NotFound(new UpdatePhotoResult
                    {
                        Message = "Photo not found or not owned by the user.",
                        Success = false
                    });
                }

                return Ok(new UpdatePhotoResult
                {
                    Message = "Photo updated successfully.",
                    Success = true,
                    PhotoId = updatedPhoto.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UpdatePhotoResult
                {
                    Message = $"Error: {ex.Message}\nStack Trace: {ex.StackTrace}",
                    Success = false
                });
            }
        }
    }

    public class UpdatePhotoRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        //public decimal? Price { get; set; }
    }

    public class UpdatePhotoResult
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int? PhotoId { get; set; }
    }
}

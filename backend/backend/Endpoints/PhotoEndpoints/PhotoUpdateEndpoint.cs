using backend.Data;
using backend.Data.Models;
using backend.Helper.Services;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos")]
    [ApiController]
    public class PhotoUpdateEndpoint(PhotoCloudinaryService photoService, IJwtService jwtService, AppDbContext db) : ControllerBase
    {
        [HttpPut("{photoId}")]
        public async Task<IActionResult> UpdatePhoto([FromRoute] int photoId, [FromForm] UpdatePhotoRequest request)
        {
            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);

            if (validationResult is UnauthorizedObjectResult unauthorizedResult)
                return unauthorizedResult;

            if (validationResult is OkObjectResult okResult)
            {
                var user = okResult.Value as User;

                // Check if the photo exists and belongs to the authenticated user
                var isUserHavePhoto = await db.Photos
                    .FirstOrDefaultAsync(p => p.Id == photoId && p.UserId == user.Id);

                if (isUserHavePhoto == null)
                {
                    return Unauthorized(new
                    {
                        IsValid = false,
                        Message = "You do not have permission to edit this photo."
                    });
                }

                if (string.IsNullOrEmpty(request.Description) && string.IsNullOrEmpty(request.Title) && string.IsNullOrEmpty(request.Location))
                {
                    return BadRequest(new UpdatePhotoResult
                    {
                        Message = "At least one field (Description, Title, or Location) must be provided to update.",
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
                    );

                    if (updatedPhoto == null)
                    {
                        return NotFound(new UpdatePhotoResult
                        {
                            Message = "Photo not found or not owned by the user.",
                            Success = false
                        });
                    }

                    return Ok(new
                    {
                        IsValid = true,
                        Message = "Photo updated successfully.",
                        Success = true,
                        PhotoId = updatedPhoto.Id,
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

            return Unauthorized(new { Message = "Invalid token." });
        }
    }

    public class UpdatePhotoRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }

    public class UpdatePhotoResult
    {
        public string? Message { get; set; }
        public bool Success { get; set; }
        public int? PhotoId { get; set; }
    }
}

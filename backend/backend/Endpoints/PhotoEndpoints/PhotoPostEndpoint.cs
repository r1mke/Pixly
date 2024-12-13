using backend.Data.Models;
using backend.Heleper.Api;
using backend.Helper.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace backend.Endpoints.PhotoEndpoints
{

    [Route("api/photos")]
    public class PhotoPostEndpoint : MyEndpointBaseAsync.WithRequest<PostPhotoRequest>.WithResult<PostPhotoResult>
    {
        private readonly PhotoService _photoService;

        public PhotoPostEndpoint(PhotoService photoService)
        {
            _photoService = photoService;
        }

      
        [HttpPost]
        public override async Task<PostPhotoResult> HandleAsync( [FromForm] PostPhotoRequest request, CancellationToken cancellationToken = default)
        {
            
            if (request.File == null || request.File.Length == 0)
            {
                return new PostPhotoResult
                {
                    Message = "File is required",
                    PhotoId = null,
                };
            }
            List<string> tags = request.Tags.Split(',').Select(tag => tag.Trim()).ToList();
            try
            {
                var photo = await _photoService.UploadPhotoAsync(
                    request.Title,
                    request.Description,
                    request.Location,
                    request.UserId,
                    request.File,
                    tags,
                    cancellationToken
                );

                return new PostPhotoResult
                {
                    Message = "Uspjesno iz endpointa",
                    PhotoId = photo.PhotoId,
                };
            }
            catch (Exception ex)
            {
                
                return new PostPhotoResult
                {
                    Message = $"Error: {ex.Message}\nStack Trace: {ex.StackTrace}",
                    PhotoId = null,
                };
            }
        }
    }

   
    public class PostPhotoRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }
        public IFormFile File { get; set; } 

        public string Tags { get; set; }
 
    }


    public class PostPhotoResult
    {
        public string? Message { get; set; }
        public int? PhotoId { get; set; }
    }


   
}

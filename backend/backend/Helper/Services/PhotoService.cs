using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using backend.Data;
using Microsoft.Extensions.Options;
using System.Drawing;  // Za određivanje orijentacije
using System.Linq;
using System.Threading.Tasks;
using backend.Data.Models;
using backend.Helper;
using System.IO;  // Za MemoryStream
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // Za IFormFile
using Newtonsoft.Json;

public class PhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly AppDbContext _context;

    public PhotoService(IOptions<CloudinarySettings> cloudinarySettings, AppDbContext context)
    {
        var account = new Account(
            cloudinarySettings.Value.CloudName,
            cloudinarySettings.Value.ApiKey,
            cloudinarySettings.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
        _context = context;
    }

    // Funkcija koja upload-uje sliku u Cloudinary koristeći IFormFile
    public async Task<PostPhotoResult> UploadPhotoAsync(string title, string description, string location, int userId, IFormFile file)
    {

        try
        {

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is required");
            }


            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var imageBytes = memoryStream.ToArray();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, new MemoryStream(imageBytes)),
                UploadPreset = "full",
                Context = new StringDictionary() { { "image_analysis", "true" } }
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult?.Error != null)
            {
                throw new Exception($"Upload failed: {uploadResult.Error.Message}");
            }

            var compressedParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, new MemoryStream(imageBytes)),
                UploadPreset = "compressed",
                Transformation = new Transformation()
                .Quality(40)
                .Overlay(new Layer().PublicId("photos/watermark/ms7qetvbuvxbmmdvstao"))
                .Gravity("center")
                .Opacity(50)
                .Crop("scale")
            };


            var compressedUploadResult = await _cloudinary.UploadAsync(compressedParams);

            if (compressedUploadResult?.Error != null)
            {
                throw new Exception($"Compressed upload failed: {compressedUploadResult.Error.Message}");
            }

            var imageOrientation = GetImageOrientation(uploadResult.Width, uploadResult.Height);


            var color = await GetDominantColorsAsync(uploadResult.PublicId);

            var photo = new Photo
            {
                Title = title,
                Description = description,
                Location = location,
                UserId = userId,
                Approved = false,
                CreateAt = DateTime.UtcNow,
                LikeCount = 0,
                ViewCount = 0,
                Price = 0,
                PhotoTags = new List<PhotoTag>(),
                Orientation = imageOrientation,
                Colors = null
            };

            // Save photo in the database
            _context.Photos.Add(photo);
            await _context.SaveChangesAsync();

            var item = new PhotoResolution
            {
                Resolution = "full_resolution",
                Url = uploadResult.Url.ToString(),
                Size = uploadResult.Bytes,
                Date = DateTime.UtcNow,
                Photo = photo

            };
            var item2 = new PhotoResolution
            {
                Resolution = "compressed",
                Url = compressedUploadResult.Url.ToString(),
                Size = compressedUploadResult.Bytes,
                Date = DateTime.UtcNow,
                Photo = photo
            };
            _context.PhotoResolutions.Add(item);
            _context.PhotoResolutions.Add(item2);

            await _context.SaveChangesAsync();



            return new PostPhotoResult
            {
                Message = $"Uspjesno iz servisa + {photo.Id}",
                PhotoId = photo.Id
            };
        }
        catch(Exception ex)
        {
            return new PostPhotoResult
            {
                PhotoId = null,
                Message = $"Error: {ex.Message}\nStack Trace: {ex.StackTrace}"
            };
        }
    }

    public class PostPhotoResult
    {
        public string? Message { get; set; }
        public int? PhotoId { get; set; }
    }


    private string GetImageOrientation(int width, int height)
    {
        if (width > height)
        {
            return "landscape";
        }
        else if (width < height)
        {
            return "portrait";
        }
        else
        {
            return "square";
        }
    }
    private async Task<string> GetDominantColorsAsync(string imagePublicId)
    {
        
        var resourceParams = new GetResourceParams(imagePublicId)
        {
            Context = true
        };

      
        var resource = await _cloudinary.GetResourceAsync(resourceParams);

        var colors = resource?.Context?.SelectToken("colors")?.ToString();

        if (!string.IsNullOrEmpty(colors))
        {
            var colorList = colors.Split(',').ToList();
            var jsonColors = JsonConvert.SerializeObject(colorList);
            return jsonColors;
        }

        return null; 
    }
}

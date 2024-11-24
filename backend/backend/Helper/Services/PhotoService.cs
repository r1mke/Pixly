using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using backend.Data;
using Microsoft.Extensions.Options;
using System.Drawing;

using backend.Data.Models;
using backend.Helper;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Services;

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

    public async Task<PostPhotoResult> UploadPhotoAsync(string title, string description, string location, int userId, IFormFile file, List<string> tags, List<Category> categories)
    {
        
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
        if (file == null || file.Length == 0) throw new ArgumentException("File is required.");
        if (tags == null || tags.Count == 0) throw new ArgumentException("At least one tag is required.");
        if (categories == null || categories.Count == 0) throw new ArgumentException("At least one category is required.");


        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();

        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, new MemoryStream(imageBytes)),
            Folder = "photos/full",
            Colors = true,
            QualityAnalysis = true
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        if (uploadResult?.Error != null)
        {
            throw new InvalidOperationException($"Upload failed: {uploadResult.Error.Message}");
        }

        var imageOrientation = GetImageOrientation(uploadResult.Width, uploadResult.Height);

        
        var transformation = imageOrientation switch
        {
            "landscape" => new Transformation().Named("landscape_transformation"),
            "portrait" => new Transformation().Named("portrait_transformation"),
            "square" => new Transformation().Named("square_transformation")
        };

        var compressedParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, new MemoryStream(imageBytes)),
            Folder = "photos/compressed",
            Transformation = transformation,
        };

        var compressedUploadResult = await _cloudinary.UploadAsync(compressedParams);
        if (compressedUploadResult?.Error != null)
        {
            throw new InvalidOperationException($"Compressed upload failed: {compressedUploadResult.Error.Message}");
        }

       
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
            Orientation = imageOrientation
        };

        var photoCategories = categories.Select(c => new PhotoCategory
        {
            Photo = photo,
            Category = c
        }).ToList();

        photo.PhotoCategories = photoCategories;

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();

       
        var fullResolution = new PhotoResolution
        {
            Resolution = "full_resolution",
            Url = uploadResult.Url.ToString(),
            Size = uploadResult.Bytes,
            Date = DateTime.UtcNow,
            Photo = photo
        };

        var compressedResolution = new PhotoResolution
        {
            Resolution = "compressed",
            Url = compressedUploadResult.Url.ToString(),
            Size = compressedUploadResult.Bytes,
            Date = DateTime.UtcNow,
            Photo = photo
        };

        _context.PhotoResolutions.AddRange(fullResolution, compressedResolution);

       
        if (uploadResult.Colors != null)
        {
            var extractedColors = uploadResult.Colors
                .Where(colorSet => float.TryParse(colorSet[1], out var percentage) && percentage > 15)
                .Select(colorSet => colorSet[0])
                .Distinct()
                .ToList();

            var existingColors = _context.Colors.ToDictionary(c => c.HexCode, c => c.Id);

            foreach (var colorHex in extractedColors)
            {
                if (!existingColors.ContainsKey(colorHex))
                {
                    var newColor = new backend.Data.Models.Color { HexCode = colorHex };
                    _context.Colors.Add(newColor);
                    await _context.SaveChangesAsync();
                    existingColors[colorHex] = newColor.Id;
                }

                var photoColor = new PhotoColor
                {
                    PhotoId = photo.Id,
                    ColorId = existingColors[colorHex]
                };

                if (!_context.PhotoColors.Any(pc => pc.PhotoId == photo.Id && pc.ColorId == existingColors[colorHex]))
                {
                    _context.PhotoColors.Add(photoColor);
                }
            }
        }

        var existingTags = _context.Tags.ToDictionary(t => t.TagName, t => t.Id);
        foreach (var tag in tags)
        {
            if (!existingTags.ContainsKey(tag))
            {
                var newTag = new Tag { TagName = tag };
                _context.Tags.Add(newTag);
                await _context.SaveChangesAsync();
                existingTags[tag] = newTag.Id;
            }

            var photoTag = new PhotoTag
            {
                PhotoId = photo.Id,
                TagId = existingTags[tag]
            };

            _context.PhotoTags.Add(photoTag);
        }


        await _context.SaveChangesAsync();

        return new PostPhotoResult
        {
            Message = $"Successfully uploaded photo with ID {photo.Id}",
            PhotoId = photo.Id
        };
    }

    public class PostPhotoResult
    {
        public string? Message { get; set; }
        public int? PhotoId { get; set; }
    }

    private string GetImageOrientation(int width, int height)
    {
        return width > height ? "landscape" : width < height ? "portrait" : "square";
    }
}

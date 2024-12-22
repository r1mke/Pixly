using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using backend.Data;
using Microsoft.Extensions.Options;
using backend.Data.Models;
using backend.Helper;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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

    public async Task<Photo> UpdatePhotoAsync(int photoId, string? title, string? description, string? location)
    {
        var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == photoId);

        if (photo == null)
        {
            return null;
        }

        /*
        if (photo.UserId != currentUserId)
        {
            return null;
        }
        */

        if (!string.IsNullOrEmpty(description)) photo.Description = description;
        if (!string.IsNullOrEmpty(title)) photo.Title = title;
        if (!string.IsNullOrEmpty(location)) photo.Location = location;
        //if (price.HasValue) photo.Price = Convert.ToInt32(price);

        await _context.SaveChangesAsync();

        return photo;
    }


    public async Task<string> UploadProfilePhotoAsync(IFormFile file, CancellationToken cancellationToken)
    {
         
        int width = 0;
        int height = 0;

        if (file != null && file.Length > 0)
        {
            using (var stream = file.OpenReadStream())
            {
                var image = await Image.LoadAsync(stream);
                width = image.Width;
                height = image.Height;    
            }
        }
        else
        {
            return "false";
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        var imageBytes = memoryStream.ToArray();


        var imageOrientation = GetImageOrientation(width, height);

        var transformation = imageOrientation switch
        {
            "landscape" => new Transformation().Named("landscapeProfile_transformation"),
            "portrait" => new Transformation().Named("portraitProfile_transformation"),
            "square" => new Transformation().Named("squareProfile_transformation"),
            _ => null
        };

        var compressedUploadResult = await UploadToCloudinaryAsync(file.FileName, imageBytes, "photos/profile", cancellationToken, transformation);
        if (compressedUploadResult.Error != null)
            throw new InvalidOperationException($"Compressed upload failed: {compressedUploadResult.Error.Message}");

        return compressedUploadResult.Url.ToString();
    } 

    public async Task<PostPhotoResult> UploadPhotoAsync(string title, string description, string location, int userId, IFormFile file, List<string> tags, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required.");
        if (tags == null || !tags.Any())
            throw new ArgumentException("At least one tag is required.");
         
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        var imageBytes = memoryStream.ToArray();

        // Upload original photo
        var uploadResult = await UploadToCloudinaryAsync(file.FileName, imageBytes, "photos/full", cancellationToken);
        if (uploadResult.Error != null)
            throw new InvalidOperationException($"Upload failed: {uploadResult.Error.Message}");

        // Determine image orientation
        var imageOrientation = GetImageOrientation(uploadResult.Width, uploadResult.Height);

        // Upload compressed photo with transformation
        var transformation = imageOrientation switch
        {
            "landscape" => new Transformation().Named("landscape_transformation"),
            "portrait" => new Transformation().Named("portrait_transformation"),
            "square" => new Transformation().Named("square_transformation"),
            _ => null
        };

        var compressedUploadResult = await UploadToCloudinaryAsync(file.FileName, imageBytes, "photos/compressed", cancellationToken, transformation);
        if (compressedUploadResult.Error != null)
            throw new InvalidOperationException($"Compressed upload failed: {compressedUploadResult.Error.Message}");

     

        // Save photo metadata to the database
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

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();

        // Save resolutions
        var resolutions = new List<PhotoResolution>
    {
        new PhotoResolution
        {
            Resolution = "full_resolution",
            Url = uploadResult.Url.ToString(),
            Size = GetSizeCategory(uploadResult.Bytes),
            Date = DateTime.UtcNow,
            Photo = photo
        },
        new PhotoResolution
        {
            Resolution = "compressed",
            Url = compressedUploadResult.Url.ToString(),
            Size = GetSizeCategory(compressedUploadResult.Bytes),
            Date = DateTime.UtcNow,
            Photo = photo
        }
       
    };
        _context.PhotoResolutions.AddRange(resolutions);

       
        if (uploadResult.Colors != null)
        {
            
            var colorList = uploadResult.Colors
                .Select(colorArray => colorArray.ToList()) 
                .ToList(); 

            await SaveColorsAsync(colorList, photo.Id, cancellationToken);
        }

    
        await SaveTagsAsync(tags, photo.Id, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return new PostPhotoResult
        {
            Message = $"Successfully uploaded photo with ID {photo.Id}",
            PhotoId = photo.Id
        };
    }

    private async Task<ImageUploadResult> UploadToCloudinaryAsync(string fileName, byte[] imageBytes, string folder, CancellationToken cancellationToken, Transformation transformation = null)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, new MemoryStream(imageBytes)),
            Folder = folder,
            Transformation = transformation,
            Colors = transformation == null,
            QualityAnalysis = transformation == null
        };
        return await _cloudinary.UploadAsync(uploadParams, cancellationToken);
    }

    private async Task SaveColorsAsync(List<List<string>> colors, int photoId, CancellationToken cancellationToken)
    {
        var extractedColors = colors
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
                await _context.SaveChangesAsync(cancellationToken);
                existingColors[colorHex] = newColor.Id;
            }

            var photoColor = new PhotoColor
            {
                PhotoId = photoId,
                ColorId = existingColors[colorHex]
            };

            if (!_context.PhotoColors.Any(pc => pc.PhotoId == photoId && pc.ColorId == existingColors[colorHex]))
            {
                _context.PhotoColors.Add(photoColor);
            }
        }
    }

    public string GetSizeCategory(long sizeInBytes)
    {
        if (sizeInBytes >= 5_000_000) 
            return "Large";
        else if (sizeInBytes >= 1_000_000)
            return "Medium";
        else
            return "Small";
    }

    private async Task SaveTagsAsync(List<string> tags, int photoId, CancellationToken cancellationToken)
    {
        var existingTags = _context.Tags.ToDictionary(t => t.TagName, t => t.Id);

        foreach (var tag in tags)
        {
            if (!existingTags.ContainsKey(tag))
            {
                var newTag = new Tag { TagName = tag };
                _context.Tags.Add(newTag);
                await _context.SaveChangesAsync(cancellationToken);
                existingTags[tag] = newTag.Id;
            }

            var photoTag = new PhotoTag
            {
                PhotoId = photoId,
                TagId = existingTags[tag]
            };

            // Provjeri da tag već nije povezan s ovom fotografijom
            if (!_context.PhotoTags.Any(pt => pt.PhotoId == photoId && pt.TagId == existingTags[tag]))
            {
                _context.PhotoTags.Add(photoTag);
            }
        }
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

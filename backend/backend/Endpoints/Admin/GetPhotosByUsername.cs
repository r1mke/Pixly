using backend.Data.Models;
using backend.Helper.DTO_s;
using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.Admin
{
    [ApiController]
    public class GetPhotosByUsername(AppDbContext db) : ControllerBase
    {
        [HttpGet("admin/user/{username}")]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
           
            var user = await db.Users
                .AsNoTracking()
                .Include(u => u.Photos)
                .ThenInclude(p => p.Resolutions)
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    UserId = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    ProfileImgUrl = u.ProfileImgUrl,
                    Photos = u.Photos.Select(p => new PhotoDTO
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Description = p.Description,
                        Price = p.Price,
                        Location = p.Location,
                        Approved = p.Approved,
                        Url = p.Resolutions
                            .Where(r => r.Resolution == "compressed")
                            .Select(r => r.Url)
                            .FirstOrDefault()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return Ok(user);
        }
    }
}

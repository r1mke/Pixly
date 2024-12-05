using backend.Data;
using backend.Data.Models;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.AuthEndpoints
{
    [Route("auth")]
    [ApiController]
    public class CurrentUserEndpoint(AppDbContext dbContext, IJwtService jwtService) : ControllerBase
    {
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, dbContext);

            if (validationResult is UnauthorizedObjectResult unauthorizedResult)
                return unauthorizedResult;

            if (validationResult is OkObjectResult okResult)
            {
                var user = okResult.Value as User;

                if (user == null) 
                    return Unauthorized(new { Message = "Invalid user data." });    

                return Ok(new
                {
                    IsValid = true,
                    User = new
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.Username,
                        IsVerified = user.IsVerified,
                        IsAdmin = user.IsAdmin,
                        ProfileImgUrl = user.ProfileImgUrl
                    }
                });
            }
            return Unauthorized(new { Message = "Invalid token." });
        }

    }
}

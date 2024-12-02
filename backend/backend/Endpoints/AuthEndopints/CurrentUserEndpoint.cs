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

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, dbContext);
            if (validationResult is UnauthorizedObjectResult unauthorizedResult)
                return unauthorizedResult;

            var user = (User)((OkObjectResult)validationResult).Value;

            return Ok(new
            {
                IsValid = true,
                Message = "Token is valid.",
                User = new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    IsVerified = user.IsVerified,
                    IsAdmin = user.IsAdmin,
                    profileImgUrl = user.ProfileImgUrl
                }
            });
        }
    }
}

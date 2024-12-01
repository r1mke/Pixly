using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using backend.Data.Models;
using backend.Helper.Services.JwtService;
using Azure;
using System.Text.Json.Serialization;

namespace backend.Endpoints.UserEndpoints
{
    [Route("user")]
    public class UserPutEndpoint(AppDbContext db, IJwtService jwtService) : ControllerBase
    {
        [HttpPut("update-user")]
        public async Task<IActionResult> HandleAsync([FromBody] UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var jwtToken = Request.Cookies["jwt"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, db);
            if (validationResult is UnauthorizedObjectResult)
            {
                var unauthorizedMessage = ((UnauthorizedObjectResult)validationResult).Value?.ToString() ?? "Unauthorized";
                return Unauthorized(new UpdateUserResponse { Message = unauthorizedMessage });
            }

            var user = (User)((OkObjectResult)validationResult).Value;

            // Retrieve the user from the database
            var existingUser = await db.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            if (existingUser == null)
                return NotFound(new UpdateUserResponse { Message = $"User with not found" });

            // Update user fields with the request values
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            existingUser.Username = request.Username;

            db.Users.Update(existingUser);
            await db.SaveChangesAsync(cancellationToken);

            return Ok(new UpdateUserResponse { Message = "User updated successfully" });
        }

        public class UpdateUserRequest
        {
            [Required]
            [MinLength(2), MaxLength(20)]
            [JsonPropertyName("firstName")]
            public string FirstName { get; set; }

            [Required]
            [MinLength(2), MaxLength(20)]
            [JsonPropertyName("lastName")]
            public string LastName { get; set; }

            [Required]
            [MinLength(5), MaxLength(20)]
            [JsonPropertyName("username")]
            public string Username { get; set; }
        }

        public class UpdateUserResponse
        {
            public string Message { get; set; }
        }
    }
}

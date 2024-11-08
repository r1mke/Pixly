using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Models.Auth;
using System.Threading;
using System.Threading.Tasks;
using backend.Heleper.Api;
using System.ComponentModel.DataAnnotations;
using static backend.Endpoints.UserEndpoints.UserPutEndpoint;
using Microsoft.IdentityModel.Tokens;

namespace backend.Endpoints.UserEndpoints
{
    [Route("api/users")]
    public class UserPutEndpoint(AppDbContext db) : MyEndpointBaseAsync
        .WithRequest<UpdateUserRequest>
        .WithResult<User>
    {

        [HttpPut("{id:int}")]
        public override async Task<User> HandleAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                throw new InvalidOperationException("Invalid request data");

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {request.Id} not found");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Username = request.Username;
            user.Email = request.Email;
            user.IsAdmin = request.IsAdmin;
            user.IsCreator = request.IsCreator;

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.Password = request.Password;

            db.Users.Update(user);
            await db.SaveChangesAsync(cancellationToken);

            return user;
        }

        
        public class UpdateUserRequest
        {
            [Required]
            public int Id { get; set; }

            [Required]
            [MinLength(2), MaxLength(20)]
            public string FirstName { get; set; }

            [Required]
            [MinLength(2), MaxLength(20)]
            public string LastName { get; set; }

            [Required]
            [MinLength(5), MaxLength(20)]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [MinLength(8), MaxLength(64)]
            public string? Password { get; set; }

            public bool IsAdmin { get; set; } = false;
            public bool IsCreator { get; set; } = false;
        }
    }
}

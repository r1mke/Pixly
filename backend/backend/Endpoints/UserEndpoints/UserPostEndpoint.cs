using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Data.Models.Auth;
using System.Threading;
using System.Threading.Tasks;
using backend.Heleper.Api;
using System.ComponentModel.DataAnnotations;
using static backend.Endpoints.UserEndpoints.UserPostEndpoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using backend.Helper;

namespace backend.Endpoints.UserEndpoints
{
    [Route("api/users")]
    public class UserPostEndpoint(AppDbContext db) : MyEndpointBaseAsync
        .WithRequest<CreateUserRequest>
        .WithResult<User>
    {
      
        [HttpPost]
        public override async Task<User> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                throw new InvalidOperationException("Invalid request data");

            var emailExists = await db.Users
               .AnyAsync(u => u.Email == request.Email, cancellationToken);

            if (emailExists)
                throw new InvalidOperationException("User with this email already exists");

            var usernameExists = await db.Users
                .AnyAsync(u => u.Username == request.Username, cancellationToken);

            if (usernameExists)
                throw new InvalidOperationException("User with this username already exists");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Password = PasswordHasher.Hash(request.Password),
                IsAdmin = request.IsAdmin,
                IsCreator = request.IsCreator
            };

            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            return user;
        }

        public class CreateUserRequest
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            [MinLength(5), MaxLength(20)]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [MinLength(8), MaxLength(64)]
            public string Password { get; set; }

            public bool IsAdmin { get; set; } = false;
            public bool IsCreator { get; set; } = false;
        }
    }
}

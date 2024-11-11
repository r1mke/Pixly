using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Data.Models.Auth;
using System.Threading;
using System.Threading.Tasks;
using static backend.Endpoints.UserEndpoints.UserPostEndpoint;
using Microsoft.EntityFrameworkCore;
using backend.Helper;
using System;
using backend.Heleper.Api;

namespace backend.Endpoints.UserEndpoints
{
    [Route("api/users")]
    public class UserPostEndpoint(AppDbContext db, IEmailSender emailSender) : MyEndpointBaseAsync
        .WithRequest<CreateUserRequest>
        .WithResult<User>
    {
        [HttpPost]
        public override async Task<User> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                throw new InvalidOperationException("Invalid request data");

            var emailExists = await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (emailExists)
                throw new InvalidOperationException("User with this email already exists");

            var usernameExists = await db.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
            if (usernameExists)
                throw new InvalidOperationException("User with this username already exists");

            var verificationCode = new Random().Next(100000, 1000000).ToString();

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Password = PasswordHasher.Hash(request.Password),
                CreatedAt = DateTime.UtcNow,
                IsVerified = false,
                RegisterCode = verificationCode
            };

            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            await emailSender.SendEmailAsync(user.Email, "Verify your email",
                $"Code to verify your email: <strong>{verificationCode}</strong>");

            return user;
        }

        public class CreateUserRequest
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using backend.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using backend.Heleper.Api;
using backend.Data.Models;
using backend.Helper.Auth.PasswordHasher;
using backend.Helper.Auth.EmailSender;
using static backend.Endpoints.AuthEndopints.UserRegisterEndpoint;
using System.ComponentModel.DataAnnotations;

namespace backend.Endpoints.AuthEndopints
{
    [Route("auth")]
    public class UserRegisterEndpoint(AppDbContext db, IEmailSender emailSender, IPasswordHasher passwordHasher) : MyEndpointBaseAsync
        .WithRequest<CreateUserRequest>
        .WithResult<UserRegistrationResponse>
    {
        [HttpPost("register")]
        public override async Task<UserRegistrationResponse> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
                throw new InvalidOperationException("Invalid request data");

            var existingUser = await db.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            var usernameExists = await db.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
            if (usernameExists)
                throw new InvalidOperationException("User with this username already exists");

            var verificationCode = new Random().Next(100000, 1000000).ToString();
            var hash = await passwordHasher.Hash(request.Password);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Username = request.Username,
                Email = request.Email,
                Password = hash,
                CreatedAt = DateTime.UtcNow,
                IsVerified = false,
                IsCreator = false,
                IsAdmin = false,
            };

            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            var newVerificationCode = new EmailVerificationCode
            {
                UserId = user.Id,
                ActivateCode = verificationCode,
                SentAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(1),
                IsUsed = false
            };

            db.EmailVerificationCodes.Add(newVerificationCode);
            await db.SaveChangesAsync(cancellationToken);

            await emailSender.SendEmailAsync(user.Email, "Verify your email",
                $"Code to verify your email: <strong>{verificationCode}</strong>");

            return new UserRegistrationResponse
            {
                User = user,
                Message = "Registration successful. A verification email has been sent, activation code will expire in 15 minutes."
            };
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
        }

        public class UserRegistrationResponse
        {
            public User User { get; set; }
            public string Message { get; set; }
        }
    }
}

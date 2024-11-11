using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Data.Models.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static backend.Endpoints.UserEndpoints.UserVerifyEmailEndpoint;
using backend.Heleper.Api;

namespace backend.Endpoints.UserEndpoints
{
    [Route("api/users")]
    public class UserVerifyEmailEndpoint(AppDbContext _db) : MyEndpointBaseAsync
        .WithRequest<VerifyEmailRequest> 
        .WithResult<string>               
    {
        [HttpPost("verify-email")]
        public override async Task<string> HandleAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.VerificationCode) || string.IsNullOrEmpty(request.Email))
                throw new InvalidOperationException("Invalid verification request.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("User not found.");
            
            if (user.RegisterCode != request.VerificationCode)
                throw new InvalidOperationException("Invalid verification code.");

            user.IsVerified = true;
            await _db.SaveChangesAsync(cancellationToken);

            return "Email successfully verified.";
        }

        public class VerifyEmailRequest
        {
            public string VerificationCode { get; set; }
            public string Email { get; set; }
        }
    }
}

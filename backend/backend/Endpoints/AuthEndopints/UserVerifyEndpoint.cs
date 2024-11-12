using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Heleper.Api;
using static backend.Endpoints.UserEndpoints.UserVerifyEmailEndpoint;

namespace backend.Endpoints.UserEndpoints
{
    [Route("auth")]
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


            if (user.IsVerified)
                throw new InvalidOperationException("User has already been verified.");

            var verificationCode = await _db.EmailVerificationCodes
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ActivateCode == request.VerificationCode, cancellationToken);

            if (verificationCode == null)
                throw new InvalidOperationException("Invalid verification code.");

            if (verificationCode.ExpiryDate < DateTime.UtcNow)
                throw new InvalidOperationException("Verification code has expired.");

            if (verificationCode.IsUsed)
                throw new InvalidOperationException("Verification code has already been used.");

            verificationCode.IsUsed = true;
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

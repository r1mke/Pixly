using backend.Data;
using backend.Heleper.Api;
using backend.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static backend.Endpoints.AuthEndopints.UserVerifyEmailEndpoint;

namespace backend.Endpoints.AuthEndopints
{
    public class UserVerifyEmailEndpoint(AppDbContext db, IJwtService jwtService) : MyEndpointBaseAsync
        .WithRequest<VerifyEmailRequest>
        .WithResult<VerificationResponse>
    {
        [HttpPost("verify-email")]
        public override async Task<VerificationResponse> HandleAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.VerificationCode) || string.IsNullOrEmpty(request.Email))
                throw new InvalidOperationException("Invalid verification request.");

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            if (user.IsVerified)
                throw new InvalidOperationException("User has already been verified.");

            var verificationCode = await db.EmailVerificationCodes
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ActivateCode == request.VerificationCode, cancellationToken);

            if (verificationCode == null)
                throw new InvalidOperationException("Invalid verification code.");

            if (verificationCode.ExpiryDate < DateTime.UtcNow)
                throw new InvalidOperationException("Verification code has expired.");

            if (verificationCode.IsUsed)
                throw new InvalidOperationException("Verification code has already been used.");

            verificationCode.IsUsed = true;
            user.IsVerified = true;
            await db.SaveChangesAsync(cancellationToken);

            var jwtToken = jwtService.GenerateJwtToken(user);

            var response = new VerificationResponse
            {
                Message = "Email successfully verified.",
                JwtToken = jwtToken
            };

            return response;
        }

        public class VerifyEmailRequest
        {
            public string VerificationCode { get; set; }
            public string Email { get; set; }
        }

        public class VerificationResponse
        {
            public string Message { get; set; }
            public string JwtToken { get; set; }
        }
    }
}
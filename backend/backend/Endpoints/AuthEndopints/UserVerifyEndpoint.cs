using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using static backend.Endpoints.AuthEndopints.UserVerifyEmailEndpoint;
using Azure;
using backend.Heleper.Api;
using System.Text.Json.Serialization;

namespace backend.Endpoints.AuthEndopints
{
    [Route("auth")]
    public class UserVerifyEmailEndpoint(AppDbContext db) : MyEndpointBaseAsync
        .WithRequest<VerifyEmailRequest>
        .WithResult<VerificationResponse>
    {
        [Authorize]
        [HttpPost("verify-email")]
        public override async Task<VerificationResponse> HandleAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.VerificationCode) || string.IsNullOrEmpty(request.Email))
                return await HandleError("Invalid verification request.", 400);

            var emailFromToken = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == emailFromToken, cancellationToken);

            if (user == null) return await HandleError("User not found or mismatched token.", 400);

            if (user.IsVerified) return await HandleError("User has already been verified.", 400);

            var verificationCode = await db.EmailVerificationCodes
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ActivateCode == request.VerificationCode, cancellationToken);

            if (verificationCode == null) return await HandleError("Invalid verification code.", 400);
            if (verificationCode.ExpiryDate < DateTime.UtcNow) return await HandleError("Verification code has expired.", 400);
            if (verificationCode.IsUsed) return await HandleError("Verification code has already been used.", 400);

            verificationCode.IsUsed = true;
            user.IsVerified = true;
            await db.SaveChangesAsync(cancellationToken);

            return new VerificationResponse { Message = "Email successfully verified." };
        }

        private async Task<VerificationResponse> HandleError(string message, int statusCode)
        {
            Response.StatusCode = statusCode;
            Console.WriteLine($"Error: {message}");
            return await Task.FromResult(new VerificationResponse { Message = message });
        }

        public class VerifyEmailRequest
        {
            [JsonPropertyName("verificationCode")]
            public string VerificationCode { get; set; }
            [JsonPropertyName("email")]
            public string Email { get; set; }
        }

        public class VerificationResponse
        {
            public string Message { get; set; }
        }
    }
}

using backend.Data;
using backend.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Auth.EmailSender;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using static backend.Endpoints.UserEndpoints.UserResendVerificationCodeEndpoint;
using Azure;
using backend.Heleper.Api;
using System.Security.Claims;

namespace backend.Endpoints.UserEndpoints
{
    [Route("auth")]
    public class UserResendVerificationCodeEndpoint(AppDbContext db, IEmailSender emailSender, IMemoryCache memoryCache) : MyEndpointBaseAsync
        .WithRequest<ResendVerificationCodeRequest>
        .WithResult<ActionResult>
    {
        private const int RequestLimit = 1;
        private const int TimeWindowInSeconds = 120;

        [Authorize]
        [HttpPost("resend-verification-code")]
        public override async Task<ActionResult> HandleAsync(ResendVerificationCodeRequest request, CancellationToken cancellationToken = default)
        {
            var emailFromToken = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(emailFromToken))
                return await HandleError("Email not found in the token.", 400);

            var cacheKey = $"resend_verification_{emailFromToken}";

            if (memoryCache.TryGetValue(cacheKey, out int requestCount))
            {
                if (requestCount >= RequestLimit)
                    return await HandleError("Too many requests. Please try again later.", 400);

                memoryCache.Set(cacheKey, requestCount + 1, TimeSpan.FromSeconds(TimeWindowInSeconds));
            }
            else
                memoryCache.Set(cacheKey, 1, TimeSpan.FromSeconds(TimeWindowInSeconds));
            

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == emailFromToken, cancellationToken);
            if (user == null)
                return await HandleError("User not found.", 400);

            if (user.IsVerified)
                return await HandleError("User is already verified.", 400);

            var verificationCode = await db.EmailVerificationCodes
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsUsed, cancellationToken);

            if (verificationCode == null || verificationCode.ExpiryDate < DateTime.UtcNow)
            {
                verificationCode = new EmailVerificationCode
                {
                    UserId = user.Id,
                    ActivateCode = new Random().Next(100000, 1000000).ToString(),
                    SentAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                    IsUsed = false
                };

                db.EmailVerificationCodes.Add(verificationCode);
                await db.SaveChangesAsync(cancellationToken);
            }

            await emailSender.SendEmailAsync(user.Email, "Verify your email",
                $"Your new verification code is: <strong>{verificationCode.ActivateCode}</strong>");

            return await HandleSuccess("Verification code has been sent.", 200);
        }

        // We use this, for clear response, not for 'Unexpected error... {MyResponse}'
        private async Task<ActionResult> HandleError(string message, int statusCode)
        {
            Response.StatusCode = statusCode;
            return new JsonResult(new { message });
        }

        private async Task<ActionResult> HandleSuccess(string message, int statusCode)
        {
            Response.StatusCode = statusCode;
            return new JsonResult(new { message });
        }

        public class ResendVerificationCodeRequest
        {
            // Since we're using the email from the token, this is no longer needed:
            public string Email { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Data.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Auth.EmailSender;
using Microsoft.Extensions.Caching.Memory; // Dodajte ovo za MemoryCache
using System.Collections.Generic;
using backend.Heleper.Api;
using static backend.Endpoints.UserEndpoints.UserResendVerificationCodeEndpoint;

namespace backend.Endpoints.UserEndpoints
{
    [Route("auth")]
    public class UserResendVerificationCodeEndpoint(AppDbContext _db, IEmailSender emailSender, IMemoryCache memoryCache) : MyEndpointBaseAsync
        .WithRequest<ResendVerificationCodeRequest>
        .WithResult<string>
    {
        private const int RequestLimit = 1; 
        private const int TimeWindowInSeconds = 120;

        [HttpPost("resend-verification-code")]
        public override async Task<string> HandleAsync(ResendVerificationCodeRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.Email))
                throw new InvalidOperationException("Email is required.");

            var cacheKey = $"resend_verification_{request.Email}";
            if (memoryCache.TryGetValue(cacheKey, out int requestCount))
            {
                if (requestCount >= RequestLimit)
                    throw new InvalidOperationException("Too many requests. Please try again later.");
                
                memoryCache.Set(cacheKey, requestCount + 1, TimeSpan.FromSeconds(TimeWindowInSeconds));
            }
            else
                memoryCache.Set(cacheKey, 1, TimeSpan.FromSeconds(TimeWindowInSeconds));
            

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            if (user.IsVerified)
                throw new InvalidOperationException("User is already verified.");

            var verificationCode = await _db.EmailVerificationCodes
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

                _db.EmailVerificationCodes.Add(verificationCode);
                await _db.SaveChangesAsync(cancellationToken);
            }

            await emailSender.SendEmailAsync(user.Email, "Verify your email",
                $"Here is your new verification code: <strong>{verificationCode.ActivateCode}</strong>");

            return "A new verification code has been sent to your email.";
        }

        public class ResendVerificationCodeRequest
        {
            public string Email { get; set; }
        }
    }
}

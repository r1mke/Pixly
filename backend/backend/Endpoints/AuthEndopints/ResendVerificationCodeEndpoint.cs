using Azure;
using backend.Data.Models;
using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Auth.EmailSender;
using backend.Helper.Services.JwtService;
using Newtonsoft.Json.Linq;

[Route("auth")]
public class ResendVerificationCodeEndpoint(AppDbContext db, IEmailSender emailSender, IMemoryCache memoryCache, IJwtService jwtService) : ControllerBase

{
    private const int RequestLimit = 1;
    private const int TimeWindowInSeconds = 120;


    [HttpPost("resend-verification-code")]
    public async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        var jwt = Request.Cookies["jwt"];

        if (string.IsNullOrEmpty(jwt))
            return await HandleError("JWT token not found in the cookies.", 400);

        if (!jwtService.IsValidJwt(jwt))
            return await HandleError("Invalid or expired token.", 401);

        var emailFromToken = jwtService.ExtractEmailFromJwt(jwt);

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
}

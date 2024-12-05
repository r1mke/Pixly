using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static backend.Endpoints.AuthEndopints.UserVerifyEmailEndpoint;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using backend.Helper.Services.JwtService;
using Azure;
using backend.Heleper.Api;
using backend.Data.Models;

namespace backend.Endpoints.AuthEndopints
{
    [Route("auth")]
    public class UserVerifyEmailEndpoint(AppDbContext db, IJwtService jwtService) : MyEndpointBaseAsync
        .WithRequest<VerifyEmailRequest>
        .WithResult<VerificationResponse>
    {
        [HttpPost("verify-email")]
        public override async Task<VerificationResponse> HandleAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(request.VerificationCode))
                return await HandleError("Invalid verification request.", 400);

            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);
            if (validationResult is UnauthorizedObjectResult)
            {
                var unauthorizedMessage = ((UnauthorizedObjectResult)validationResult).Value?.ToString() ?? "Unauthorized";
                return await HandleError(unauthorizedMessage, 401);
            }

            var user = (User)((OkObjectResult)validationResult).Value;

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
            return await Task.FromResult(new VerificationResponse { Message = message });
        }

        public class VerifyEmailRequest
        {
            [JsonPropertyName("verificationCode")]
            public string VerificationCode { get; set; }
        }

        public class VerificationResponse
        {
            public string Message { get; set; }
        }
    }
}

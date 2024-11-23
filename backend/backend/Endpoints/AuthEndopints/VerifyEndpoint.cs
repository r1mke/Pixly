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
            if (string.IsNullOrEmpty(jwtToken))
                return await HandleError("Authorization token missing in cookies.", 401);

            if (!jwtService.IsValidJwt(jwtToken))
                return await HandleError("Invalid or expired token.", 401);

            var emailFromToken = jwtService.ExtractEmailFromJwt(jwtToken);
            if (string.IsNullOrEmpty(emailFromToken))
                return await HandleError("Invalid token or missing email in token.", 401);

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

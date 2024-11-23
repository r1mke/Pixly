using Microsoft.AspNetCore.Mvc;
using backend.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Authorization;

namespace backend.Endpoints.AuthEndopints
{
    [AllowAnonymous]
    [Route("auth")]
    public class VerifyTokenEndpoint(AppDbContext db, IJwtService jwtService) : ControllerBase
    {
        [HttpGet("verify-token")]
        public async Task<VerifyTokenResponse> VerifyTokenAsync(CancellationToken cancellationToken = default)
        {
            var jwtToken = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(jwtToken))
                return new VerifyTokenResponse { IsValid = false, Message = "Token is missing in cookies." };

            if (!jwtService.IsValidJwt(jwtToken))
                return new VerifyTokenResponse { IsValid = false, Message = "Invalid or expired token." };

            var email = jwtService.ExtractEmailFromJwt(jwtToken);
            if (string.IsNullOrEmpty(email))
                return new VerifyTokenResponse { IsValid = false, Message = "Invalid token structure." };

            var userExists = await db.Users.AnyAsync(u => u.Email == email, cancellationToken);
            if (!userExists)
                return new VerifyTokenResponse { IsValid = false, Message = "User not found." };

            return new VerifyTokenResponse
            {
                IsValid = true,
                Message = "Token is valid."
            };
        }

        public class VerifyTokenResponse
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }
    }
}

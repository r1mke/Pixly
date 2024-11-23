using Microsoft.AspNetCore.Mvc;
using backend.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Services.JwtService;
using backend.Data.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace backend.Endpoints.AuthEndopints
{
    [AllowAnonymous]
    [Route("auth")]
    public class RefreshTokenEndpoint(AppDbContext db, IJwtService jwtService) : ControllerBase
    {
        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            var jwtToken = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(jwtToken) || !jwtService.IsValidJwt(jwtToken))
                return Unauthorized(new { Message = "Invalid or missing JWT token." });

            var email = jwtService.ExtractEmailFromJwt(jwtToken);
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { Message = "Invalid JWT token structure." });

            var userExists = await db.Users.AnyAsync(u => u.Email == email, cancellationToken);
            if (!userExists)
                return Unauthorized(new { Message = "User not found." });

            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new { Message = "Refresh token is missing" });

            var storedRefreshToken = await db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

            if (storedRefreshToken == null || storedRefreshToken.IsExpired || storedRefreshToken.IsRevoked)
                return BadRequest(new { Message = "Invalid or expired refresh token." });

            storedRefreshToken.RevokedAt = DateTime.UtcNow;
            db.RefreshTokens.Update(storedRefreshToken);
            await db.SaveChangesAsync(cancellationToken);

            var newRefreshToken = new RefreshToken
            {
                UserId = storedRefreshToken.UserId,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
                CreatedAt = DateTime.UtcNow
            };
            db.RefreshTokens.Add(newRefreshToken);
            await db.SaveChangesAsync(cancellationToken);

            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == storedRefreshToken.UserId, cancellationToken);
            if (user == null)
                return Unauthorized(new { Message = "User not found." });

            var newJwtToken = jwtService.GenerateJwtToken(user);

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,
            });

            return Ok(new RefreshTokenResponse
            {
                JwtToken = newJwtToken,
                Message = "New JWT token has been generated and refresh token has been updated."
            });
        }


        public class RefreshTokenResponse
        {
            public string JwtToken { get; set; }
            public string Message { get; set; }
        }
    }
}

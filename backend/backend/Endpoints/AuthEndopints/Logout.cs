using Azure;
using backend.Data.Models;
using backend.Data;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LogoutEndpoint;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Auth.PasswordHasher;
using backend.Heleper.Api;
using Newtonsoft.Json.Linq;

[Route("auth")]
public class LogoutEndpoint(AppDbContext db, IPasswordHasher passwordHasher, IJwtService jwtService) : ControllerBase
{
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var token = Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token) || !jwtService.IsValidJwt(token))
            return Unauthorized("Invalid or expired token");

        var email = jwtService.ExtractEmailFromJwt(token);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user == null)
            return Unauthorized("User not found");

        jwtService.Logout(user.Id);

        Response.Cookies.Delete("jwt", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/" // Dodaj ovu opciju
        });


        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/" // Dodaj ovu opciju
        });


        return Ok(new { message = "Successfully logged out" });
    }
}

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
        var jwtToken = Request.Cookies["jwt"];

        var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, db);
        if (validationResult is UnauthorizedObjectResult unauthorizedResult)
            return unauthorizedResult;

        var user = (User)((OkObjectResult)validationResult).Value;

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

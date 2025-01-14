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
using backend.Heleper.Api;
using Newtonsoft.Json.Linq;
using backend.Helper.Services;
using backend.Helper.Services.PasswordHasher;

[Route("auth")]
public class LogoutEndpoint(AppDbContext db, IPasswordService passwordHasher, IJwtService jwtService, AuthService authService) : ControllerBase
{
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var jwtToken = Request.Cookies["jwt"];
        var refreshToken = Request.Cookies["refreshToken"];

        var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);
        if (validationResult is UnauthorizedObjectResult unauthorizedResult)
            return unauthorizedResult;

        var user = (User)((OkObjectResult)validationResult).Value;

        jwtService.Logout(user.Id);

        authService.DeleteJwtCookie();
        authService.DeleteRefreshToken();

        return Ok(new { message = "Successfully logged out" });
    }
}

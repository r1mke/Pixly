using backend.Data;
using backend.Helper.Services;
using backend.Helper.Services.JwtService;
using Microsoft.EntityFrameworkCore;

public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public JwtRefreshMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        var jwtToken = context.Request.Cookies["jwt"];
        var refreshToken = context.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            await _next(context);
            return;
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Find the refresh token in the database
            var refreshTokenRecord = await dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (refreshTokenRecord == null || !refreshTokenRecord.IsActive)
            {
                await _next(context);
                return;
            }

            // Check if the JWT is valid
            if (!string.IsNullOrEmpty(jwtToken) && jwtService.IsValidJwt(jwtToken))
            {
                // If JWT is valid, continue the request
                var principal = jwtService.GetPrincipalFromToken(jwtToken);
                if (principal != null)
                    context.User = principal;
                

                await _next(context);
                return;
            }

            // If JWT is expired, refresh the token using the refresh token
            var newJwtToken = jwtService.GenerateJwtToken(refreshTokenRecord.User);

            // Set the new JWT in the cookies
            authService.SetJwtCookie(newJwtToken);

            // Set the user in the context
            var newPrincipal = jwtService.GetPrincipalFromToken(newJwtToken);
            if (newPrincipal != null)
                context.User = newPrincipal;
            
        }

        // Continue processing the request
        await _next(context);
    }

}

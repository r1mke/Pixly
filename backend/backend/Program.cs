using backend.Data;
using backend.Helper.Auth.EmailSender;
using backend.Helper.Services.JwtService;
using backend.Helper.String;
using backend.Helper;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using backend.Helper.Auth.PasswordHasher;
using backend.Helper.Services;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Env.Load();

        builder.Configuration.AddJsonFile("appsettings.json");

        // Preko environment varijabli zamijenite vrijednosti u konfiguraciji
        builder.Configuration["Smtp:Username"] = Environment.GetEnvironmentVariable("SMTP_USERNAME");
        builder.Configuration["Smtp:Password"] = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        builder.Configuration["Smtp:FromEmail"] = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL");

        builder.Configuration["CloudinarySettings:CloudName"] = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
        builder.Configuration["CloudinarySettings:ApiKey"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
        builder.Configuration["CloudinarySettings:ApiSecret"] = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

        builder.Configuration["Jwt:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

        builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        builder.Services.AddMemoryCache();

        builder.Services.AddDistributedMemoryCache(); // For session storage
        builder.Services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true; // Ensures session works even when cookies are disabled
            options.IdleTimeout = TimeSpan.FromMinutes(3); // Session timeout
        });


        // Autentifikacija putem cookies
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "YourAppName_AuthCookie";  // Ime kolačića
                //options.LoginPath = "/login";  // Putanja na kojoj ćeš upravljati loginom
                //options.LogoutPath = "/logout";  // Putanja za logout
                options.AccessDeniedPath = "/access-denied";  // Putanja za pristup koji nije dozvoljen
                options.SlidingExpiration = true;  // Automatsko obnavljanje isteka sesije
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Vrijeme trajanja kolačića
            });

        builder.Services.AddAuthorization();

        // Registracija servisa
        builder.Services.AddScoped<IEmailSender, EmailSender>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<PhotoService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IStringHelper, StringHelper>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Swagger konfiguracija (ako koristiš za dokumentaciju)

        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new string[] {}
                }
            });
        });

        // Povezivanje s bazom podataka
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Cloudinary postavke (ako se koristi)
        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

        var app = builder.Build();

        // CORS konfiguracija
        app.UseCors(options => options
             .SetIsOriginAllowed(x => _ = true) // Ovo omogućava sve izvore
             .AllowCredentials()
             .AllowAnyMethod()
             .AllowAnyHeader()
        );

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        // Middleware za autentifikaciju i autorizaciju
        app.UseMiddleware<JwtRefreshMiddleware>();
        app.UseSession(); // This must be added before UseAuthentication or UseAuthorization

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

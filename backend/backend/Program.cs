using backend.Data;
using backend.Helper.Auth.EmailSender;
using backend.Helper;
using Microsoft.EntityFrameworkCore;
using backend.Helper.Auth.PasswordHasher;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using backend.Helper.String;
using backend.Helper.Services.JwtService;
using DotNetEnv;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Env.Load(); // Učitaj varijable iz .env fajla

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

        // Autentifikaciju i JWT validacija
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Postaviti na true u produkciji
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });

        builder.Services.AddAuthorization();

        // Registracija servisa
        builder.Services.AddScoped<IEmailSender, EmailSender>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<PhotoService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IStringHelper, StringHelper>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Swagger s Bearer authentication
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
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials()
        );

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // Middleware za autentifikaciju i autorizaciju
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

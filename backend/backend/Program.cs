using backend.Data;
using backend.Helper.Auth.EmailSender;
using backend.Helper.Auth.PasswordHasher;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMemoryCache();

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Add services for MVC Controllers (API)
            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add DbContext with SQL Server configuration
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Add authorization middleware
            app.UseAuthorization();

            // Map controllers to routes
            app.MapControllers();

            // Start the application
            app.Run();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Data.Models;
using System.Threading;
using System.Threading.Tasks;
using backend.Heleper.Api;
using Microsoft.EntityFrameworkCore;
using static backend.Endpoints.UserEndpoints.UserGetByIdEndpoint;

namespace backend.Endpoints.UserEndpoints
{
    [Route("api/users")]
    public class UserGetByIdEndpoint(AppDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithResult<UserGetByIdResponse>
    {
       
        [HttpGet("{id:int}")]
        public override async Task<UserGetByIdResponse> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);


            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            var response = new UserGetByIdResponse
            {
                ID = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
                IsCreator = user.IsCreator
            };

            return response;
        }

        public class UserGetByIdResponse
        {
            public required int ID { get; set; }
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public required string Username { get; set; }
            public required string Email { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsCreator { get; set; }
        }
    }
}

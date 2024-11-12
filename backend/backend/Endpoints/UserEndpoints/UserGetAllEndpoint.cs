using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Data.Models;
using System.Threading;
using System.Threading.Tasks;
using backend.Heleper.Api;
using static backend.API.Endpoints.UserEndpoints.UserGetAllEndpoint;

namespace backend.API.Endpoints.UserEndpoints
{
    [Route("api/users")]
    public class UserGetAllEndpoint(AppDbContext db) : MyEndpointBaseAsync
        .WithoutRequest
        .WithResult<UserGetAllResponse[]>
    {
        [HttpGet]
        public override async Task<UserGetAllResponse[]> HandleAsync(CancellationToken cancellationToken = default)
        {
            var users = await db.Users
                .AsNoTracking()
                .Select(u => new UserGetAllResponse
                {
                    ID = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    IsAdmin = u.IsAdmin,
                    IsCreator = u.IsCreator
                })
                .ToArrayAsync(cancellationToken);

            return users;
        }

        public class UserGetAllResponse
        {
            public required int ID { get; set; }
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public required string Email { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsCreator { get; set; }
        }
    }
}

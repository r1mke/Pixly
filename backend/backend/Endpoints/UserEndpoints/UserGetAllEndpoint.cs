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
        .WithRequest<UserGetAllRequest>
        .WithResult<UserGetAllResponse[]>
    {
        [HttpGet]
        public override async Task<UserGetAllResponse[]> HandleAsync([FromQuery] UserGetAllRequest request,CancellationToken cancellationToken = default)
        {
            if (request.query == null)
            {
                var users = await db.Users
                    .AsNoTracking()
                    .Select(u => new UserGetAllResponse
                    {
                        ID = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Username = u.Username,
                        Email = u.Email,
                        ProfileImgUrl = u.ProfileImgUrl,
                        IsAdmin = u.IsAdmin,
                        IsCreator = u.IsCreator
                    })
                    .ToArrayAsync(cancellationToken);

            return users;

            }
            else {
                var resultAuthors = await db.Users
               .Where(u => u.FirstName.ToLower().Contains(request.query.ToLower()) || u.LastName.ToLower().Contains(request.query.ToLower()) || u.Username.ToLower().Contains(request.query.ToLower()))
               .Select(u => new UserGetAllResponse
                    {
                        ID = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Username = u.Username,
                        Email = u.Email,
                        ProfileImgUrl = u.ProfileImgUrl,
                        IsAdmin = u.IsAdmin,
                        IsCreator = u.IsCreator
                    })
                    .ToArrayAsync(cancellationToken);
                return resultAuthors;
            }

        }


        public class UserGetAllResponse
        {
            public required int ID { get; set; }
            public required string FirstName { get; set; }
            public required string LastName { get; set; }
            public string Username { get; set; }
            public required string Email { get; set; }
            public string? ProfileImgUrl { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsCreator { get; set; }
        }
    }

    public class UserGetAllRequest
    {
        public string? query { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Data.Models;
using System.Threading;
using System.Threading.Tasks;
using backend.Heleper.Api;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.UserEndpoints
{
    [Route("api/users")]
    public class UserDeleteEndpoint(AppDbContext db) : MyEndpointBaseAsync
        .WithRequest<int>
        .WithResult<bool>
    {
  
        [HttpDelete("{id:int}")]
        public override async Task<bool> HandleAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
   
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found");

            db.Users.Remove(user);
            await db.SaveChangesAsync(cancellationToken);
            
            return true;
        }
    }
}

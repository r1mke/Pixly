using backend.Heleper.Api;

using Microsoft.AspNetCore.Mvc;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/test")]
    public class Test : MyEndpointBaseAsync.WithRequest<string>.WithResult<string>
    {

        [HttpPost]
        public override Task<string> HandleAsync(string request, CancellationToken cancellationToken = default)
        {
            var poruka = request.ToString();
            return Task.FromResult($"Poruka je {poruka}");
        }
    }
}

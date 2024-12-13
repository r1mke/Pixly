using backend.Data;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.PhotoEndpoints
{
    [Route("api/photos/search/suggestions")]
    public class PhotoSearchSuggestion : MyEndpointBaseAsync.WithRequest<PhotoSearchSuggestionRequest>.WithResult<PhotoSearchSuggestionResult>

    {
        private readonly AppDbContext _db;

        public PhotoSearchSuggestion(AppDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public override async Task<PhotoSearchSuggestionResult> HandleAsync( [FromQuery] PhotoSearchSuggestionRequest request, CancellationToken cancellationToken = default)
        {

            var result = await _db.Tags
             .Where(t => t.TagName.StartsWith(request.Title))
             .Select(t => t.TagName.ToString())
             .Distinct()
             .ToListAsync(cancellationToken);

           ;


            return new PhotoSearchSuggestionResult
            {
                Suggestions = result.ToArray()
            };
        }
    }

    public class PhotoSearchSuggestionResult
    {
        public string[] Suggestions { get; set; }
    }

    public class PhotoSearchSuggestionRequest
    {
        public string Title { get; set; }
    }
}

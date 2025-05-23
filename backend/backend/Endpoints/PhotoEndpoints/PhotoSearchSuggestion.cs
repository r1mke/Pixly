﻿using backend.Data;
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

            var resultPhotos = await _db.Tags
             .Where(t => t.TagName.ToLower().StartsWith(request.Title.ToLower()))
             .Select(t => t.TagName)
             .Distinct()
             .Take(5)
             .ToArrayAsync(cancellationToken);

           var resultAuthors = await _db.Users
                .Where(u=>u.FirstName.ToLower().Contains(request.Title.ToLower()) ||  u.LastName.ToLower().Contains(request.Title.ToLower()) || u.Username.ToLower().Contains(request.Title.ToLower()))
                .Select(u=>u.Username)
                .Distinct()
                .Take(5)
                .ToArrayAsync(cancellationToken);


            return new PhotoSearchSuggestionResult
            {
                SuggestionsPhotos = resultPhotos,
                SuggestionsAuthors = resultAuthors
            };
        }
    }

    public class PhotoSearchSuggestionResult
    {
        public string[] SuggestionsPhotos { get; set; }
        public string[] SuggestionsAuthors { get; set; }
    }

    public class PhotoSearchSuggestionRequest
    {
        public string Title { get; set; }
    }
}

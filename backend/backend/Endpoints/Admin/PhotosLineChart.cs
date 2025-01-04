using backend.Data;
using backend.Heleper.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints.Admin
{
    [Route("api/photos/linechart")]
    public class PhotosLineChart : MyEndpointBaseAsync.WithoutRequest.WithResult<PhotosLineChartResult>
    {
        private readonly AppDbContext _dbContext;

        public PhotosLineChart(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public override async Task<PhotosLineChartResult> HandleAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.Now.Date;
            var lastWeek = today.AddDays(-6);

            // Grupisanje broja fotografija po datumu
            var photosPerDay = await _dbContext.Photos
                .Where(photo => photo.CreateAt.Date >= lastWeek && photo.CreateAt.Date <= today)
                .GroupBy(photo => photo.CreateAt.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    Count = group.Count()
                })
                .ToDictionaryAsync(x => x.Date, x => x.Count, cancellationToken);

            // Grupisanje broja korisnika po datumu
            var usersPerDay = await _dbContext.Users
                .Where(user => user.CreatedAt.Date >= lastWeek && user.CreatedAt.Date <= today)
                .GroupBy(user => user.CreatedAt.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    Count = group.Count()
                })
                .ToDictionaryAsync(x => x.Date, x => x.Count, cancellationToken);

            // Kreiranje nizova za posljednjih 7 dana
            var photoCounts = Enumerable.Range(0, 7)
                .Select(offset => today.AddDays(-offset))
                .OrderBy(date => date)
                .Select(date => photosPerDay.TryGetValue(date, out var count) ? count : 0)
                .ToArray();

            var userCounts = Enumerable.Range(0, 7)
                .Select(offset => today.AddDays(-offset))
                .OrderBy(date => date)
                .Select(date => usersPerDay.TryGetValue(date, out var count) ? count : 0)
                .ToArray();

            return new PhotosLineChartResult
            {
                photosPerDay = photoCounts,
                usersPerDay = userCounts
            };
        }
    }

    public class PhotosLineChartResult
    {
        public int[] photosPerDay { get; set; }
        public int[] usersPerDay { get; set; }
    }
}

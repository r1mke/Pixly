using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly StripeService _stripeService;
    private readonly AppDbContext _db;

    public StripeController(StripeService stripeService, AppDbContext db)
    {
        _stripeService = stripeService;
        _db = db;
    }

    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        try
        {
            var session = _stripeService.CreateCheckoutSession(
                request.Amount,
                request.Currency,
                request.PhotoImage,
                request.PhotoDescription,
                request.SuccessUrl,
                request.CancelUrl
            );

            return Ok(new { sessionId = session.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("download-photo/{id}")]
    public async Task<IActionResult> DownloadPhoto([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var photo = await _db.PhotoResolutions
            .Where(p => p.PhotoId == id && p.Resolution == "full_resolution")
            .FirstOrDefaultAsync(cancellationToken);

        if (photo == null || string.IsNullOrEmpty(photo.Url))
            return NotFound();
        

        try
        {
            string imageUrl = photo.Url;

            using (var httpClient = new HttpClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                var fileName = Path.GetFileName(imageUrl);

                return File(imageBytes, "image/jpg", fileName);
            }
        }
        catch (Exception ex)
        {
            return BadRequest("Greška prilikom preuzimanja slike: " + ex.Message);
        }
    }

}

public class CheckoutRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string SuccessUrl { get; set; }
    public string CancelUrl { get; set; }
    public string PhotoImage { get; set; }
    public string PhotoDescription { get; set; }
}

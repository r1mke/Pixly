using backend.Data;
using backend.Data.Models;
using backend.Helper.Services.JwtService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using static backend.Endpoints.AuthEndopints.UserVerifyEmailEndpoint;

[ApiController]
[Route("api/[controller]")]
public class StripeController(StripeService stripeService, AppDbContext db, IJwtService jwtService) : ControllerBase
{
    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] CheckoutRequest request)
    {
        try
        {
            var session = stripeService.CreateCheckoutSession(
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
        var photo = await db.PhotoResolutions
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


    [HttpPost("save-payment")]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
    {
        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(request.SessionId);

            if (session.PaymentStatus != "paid")
                return BadRequest(new { IsValid = false, status = "Payment Failed" });

            var jwtToken = Request.Cookies["jwt"];
            var refreshToken = Request.Cookies["refreshToken"];

            var validationResult = await jwtService.ValidateJwtAndUserAsync(jwtToken, refreshToken, db);
            if (validationResult is UnauthorizedObjectResult unauthorizedResult)
            {
                var message = unauthorizedResult.Value?.ToString() ?? "Unauthorized";
                return Unauthorized(new { error = message });
            }

            var user = (User)((OkObjectResult)validationResult).Value;

            var existingTransaction = await db.Transactions
                .AnyAsync(t => t.UserId == user.Id && t.PhotoId == request.PhotoId);

            if (existingTransaction)
                return BadRequest(new { IsValid = true, status = "Transaction already exists." });

            //If transaction doesn't exist, save them
            var transaction = new Transaction
            {
                UserId = user.Id,
                PhotoId = request.PhotoId,
                Amount = request.Amount,
                Currency = "USD",
                PlatformEarning = request.Amount * 0.20m,
                AuthorEarning = request.Amount * 0.80m,
                SessionId = request.SessionId,
                PaymentStatus = "paid",
                CreatedAt = DateTime.UtcNow
            };

            db.Transactions.Add(transaction);
            await db.SaveChangesAsync();

            return Ok(new { IsValid = true, status = "Payment confirmed and stored." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
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


public class ConfirmPaymentRequest
{
    public int PhotoId { get; set; }
    public decimal Amount { get; set; }
    public string SessionId { get; set; }
}


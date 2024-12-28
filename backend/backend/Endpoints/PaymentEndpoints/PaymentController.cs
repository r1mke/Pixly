using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

[ApiController]
[Route("api/[controller]")]
public class StripeController : ControllerBase
{
    private readonly StripeService _stripeService;

    public StripeController(StripeService stripeService)
    {
        _stripeService = stripeService;
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

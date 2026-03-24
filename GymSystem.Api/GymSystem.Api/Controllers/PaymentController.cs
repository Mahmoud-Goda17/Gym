using Gym.Application.Dtos.Payment;
using Gym.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController(IPaymentService service) : ApiController
    {
        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment(Guid memberId, Guid membershipid, CancellationToken ct)
        {
            var result = await service.CreateMembershipPayment(memberId, membershipid, ct);
            return result.Match(
                _ => Ok(),
                errors => Problem(errors));
        }
        [HttpGet("HandleWebhook")]
        public async Task<IActionResult> HandleWebhook(FawaterkWebhookDto dto, CancellationToken ct)
        {
            await service.HandleWebhook(dto, ct);
            return Ok();
        }
    }
}

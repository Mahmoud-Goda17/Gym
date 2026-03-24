
using System.Net.Http.Json;
using Gym.Application.Dtos.Payment;
using Gym.Application.Errors;
using Gym.Application.Interfaces;
using Gym.Application.Mapper.Payment;
using Gym.Domain.Payment;
using Gym.Domain.Payment.Enums;
using Gym.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gym.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAppDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentService(
            IAppDbContext context,
            ILogger<PaymentService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<Result<string>> CreateMembershipPayment(
            Guid memberId,
            Guid membershipId,
            CancellationToken ct)
        {
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == memberId, ct);

            if (member == null)
            {
                _logger.LogWarning("Member {MemberId} not found", memberId);
                return AplicationErrors.NotFound("Member", memberId);
            }
            var membership = await _context.MemberShips
                .FirstOrDefaultAsync(m => m.Id == membershipId , ct);
            if (membership == null)
            {
                _logger.LogWarning("Membership {MembershipId} not found", membershipId);
                return AplicationErrors.NotFound("Membership", membershipId);
            }
                decimal membershipPrice = SetMemnbershipValue.GetMembershipValue(membership.Type); 

            var payment = Payment.Create(memberId,membershipId, membershipPrice);
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(ct);

           var client = _httpClientFactory.CreateClient("Fawaterk");

            var requestBody = new
            {
                amount = membershipPrice,
                currency = "EGP",
                customer = new
                {
                    name = member.FullName,
                    email = member.Email,
                    phone = member.Phone
                },
                invoice_id = payment.Id.ToString(),
                success_url = "https://yourdomain.com/success",
                fail_url = "https://yourdomain.com/fail"
            };

            var response = await client.PostAsJsonAsync(
                "createInvoice",
                requestBody,
                ct);

            if (!response.IsSuccessStatusCode)
            {
                payment.MarkAsFailed();
                await _context.SaveChangesAsync(ct);

               _logger.LogError("Fawaterk API failed for Payment {PaymentId}", payment.Id);
                return PaymentErrors.ExternalServiceFailed;
            }

            var result = await response.Content.ReadFromJsonAsync<FawaterkResponse>(ct);

            payment.SetExternalPaymentId(result!.data.invoice_id);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Payment {PaymentId} created successfully", payment.Id);

            return result.data.payment_url;
        }
        public async Task HandleWebhook(FawaterkWebhookDto dto,CancellationToken ct)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p =>
                    p.ExternalPaymentId == dto.invoice_id, ct);

            if (payment == null)
                return;

            if (payment.Status == PaymentStatus.Paid)
                return;
            if (dto.amount != payment.Amount)
            {
                _logger.LogWarning("Amount mismatch for Payment {PaymentId}", payment.Id);
                return;
            }


            if (dto.status.Equals("paid", StringComparison.OrdinalIgnoreCase))
            {
                payment.MarkAsPaid();

                var membership = await _context.MemberShips
                    .FirstOrDefaultAsync(m => m.Id == payment.MembershipId, ct);
                if (membership != null)
                {
                    membership.DeFreezMemberShip();
                    _logger.LogInformation("Membership {MembershipId} activated for Payment {PaymentId}", membership.Id, payment.Id);
                }
                _logger.LogInformation("Payment {PaymentId} confirmed", payment.Id);
            }
            else
            {
                payment.MarkAsFailed();
            }

            await _context.SaveChangesAsync(ct);
        }

    }
}

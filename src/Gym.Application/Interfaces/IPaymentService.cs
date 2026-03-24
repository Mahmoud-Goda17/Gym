
using Gym.Application.Dtos.Payment;
using Gym.Domain.Results;

namespace Gym.Application.Interfaces
{
    public interface IPaymentService
    {
       Task<Result<string>> CreateMembershipPayment(
            Guid memberId,
            Guid membershipId,
            CancellationToken ct);
        Task HandleWebhook(FawaterkWebhookDto dto,CancellationToken ct);
    }
}

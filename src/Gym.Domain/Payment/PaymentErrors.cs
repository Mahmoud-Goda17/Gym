
using Gym.Domain.Results;

namespace Gym.Domain.Payment
{
    public static class PaymentErrors
    {
        public static Error ExternalServiceFailed =>
            Error.Unexpected("Payment.ExternalServiceFailed", "Failed to process payment with external service.");
    }
}

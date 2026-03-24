
using Gym.Domain.Payment.Enums;

namespace Gym.Domain.Payment
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid MemberId { get; private set; }
        public Guid MembershipId { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = "EGP";
        public PaymentStatus Status { get; private set; }
        public string ExternalPaymentId { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; private set; }

        private Payment() { }

        public static Payment Create(Guid memberId,Guid membersipId, decimal amount)
        {
            return new Payment
            {
                Id = Guid.NewGuid(),
                MemberId = memberId,
                MembershipId = membersipId,
                Amount = amount,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTimeOffset.UtcNow
            };
        }

        public void SetExternalPaymentId(string id)
        {
            if (!string.IsNullOrWhiteSpace(ExternalPaymentId))
                throw new InvalidOperationException("ExternalPaymentId already set");

            ExternalPaymentId = id;
        }

        public void MarkAsPaid()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Invalid state transition");

            Status = PaymentStatus.Paid;
        }

        public void MarkAsFailed()
            => Status = PaymentStatus.Failed;
    }
}

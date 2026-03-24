
using Gym.Domain.Common;
using Gym.Domain.Members.Memberships.Enums;
using Gym.Domain.Results;

namespace Gym.Domain.Members.Memberships
{
    public sealed class MemberShip: AuditableEntity
    {
        public MembershipType Type { get; private set; }
        public DateTimeOffset StartDate { get; private set; }
        public DateTimeOffset EndDate { get; private set; }
        public MembershipStatus Status { get; private set; }
        public bool IsFrozen { get; private set; }
        private readonly List<Payment.Payment> _payments = [];
        public IEnumerable<Payment.Payment> Payments => _payments.AsReadOnly();

        private MemberShip()
        {
        }
        internal MemberShip(Guid id, MembershipType type, DateTimeOffset startDate, DateTimeOffset endDate)
            : base(id)
        {
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Status = MembershipStatus.Frozen;
            IsFrozen = true;
        }

        public Result<Updated> Update(MembershipType type, DateTimeOffset startDate, DateTimeOffset endDate, MembershipStatus status)
        {
            if (endDate <= startDate)
            {
                return MemberShipErrors.InvalidTimming;
            }
            if (!Enum.IsDefined(type))
            {
                return MemberShipErrors.InvalidMembershipType;
            }
            if (!Enum.IsDefined(status))
            {
                return MemberShipErrors.InvalidMembershipStatus;
            }
            Type = type;
            StartDate = startDate;
            EndDate = endDate;
            Status = status;
            return Result.Updated;
        }
        public Result<Updated> freezMembership()
        {
           if(IsFrozen)
            {
                return MemberShipErrors.AlreadyFrozen;
            }
           IsFrozen = true;
           Status= MembershipStatus.Frozen;
            return Result.Updated;
        }
        public Result<Updated> DeFreezMemberShip()
        {
            if(!IsFrozen)
            {
                return MemberShipErrors.AlreadyNotFrozen;
            }
            IsFrozen = false;
            Status= MembershipStatus.Active;
            return Result.Updated;
        }
        public Result<Updated> Expire()
        {
            if (Status != MembershipStatus.Active)
                return MemberShipErrors.InvalidMembershipStatus;

            Status = MembershipStatus.Expired;
            return Result.Updated;
        }

    }
}


using Gym.Domain.ClassBooking.Enums;
using Gym.Domain.Common;
using Gym.Domain.GymClasss;
using Gym.Domain.Members;
using Gym.Domain.Results;

namespace Gym.Domain.ClassBooking
{
    public sealed class ClassBooking:AuditableEntity
    {
        public Guid MemberId { get; private set; }
        public Member? member { get;  set; }
        public Guid ClassId { get; private set; }
        public GymClass? gymClass { get; set; }
        public BookingStatus Status { get; private set; }
        public DateTimeOffset BookedAt { get; private set; }
        private ClassBooking() { }
        private ClassBooking(Guid id, Guid memberId, Guid classId)
            : base(id)
        {
            MemberId = memberId;
            ClassId = classId;
            Status = BookingStatus.Active;
            BookedAt = DateTimeOffset.UtcNow;
        }
        public static Result<ClassBooking> Create(Guid id, Guid memberId, Guid classId)
        {
            if (memberId == Guid.Empty)
            {
                return ClassBookingErrors.MemberIdRequired;
            }
            if (classId == Guid.Empty)
            {
                return ClassBookingErrors.ClassIdRequired;
            }
            return new ClassBooking(id, memberId, classId);
        }
        public Result<Updated> Canceld()
        {
            if (Status == BookingStatus.Cancelled)
            {
                return ClassBookingErrors.AlreadyCancelled;
            }
            Status = BookingStatus.Cancelled;
            return Result.Updated;
        }

    }
}

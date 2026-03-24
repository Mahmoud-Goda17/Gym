
using Gym.Domain.Common;
using Gym.Domain.Members.Attendances.Enums;
using Gym.Domain.Results;

namespace Gym.Domain.Members.Attendances
{
    public sealed class Attendance : AuditableEntity
    {
        public Guid MeberId { get; private set; }
        public DateTimeOffset CheckInTime { get; private set; }
        public DateTimeOffset? CheckOutTime { get; private set; }
        public Status status { get; private set; }
        private Attendance()
        {
        }
        internal Attendance(Guid id, Guid memberId, DateTimeOffset checkInTime)
            : base(id)
        {
            MeberId = memberId;
            CheckInTime = checkInTime;
            status = Status.CheckedIn;
        } 
        public Result<Updated> CheckOut(DateTimeOffset checkOutTime)
        {
            if (status == Status.CheckedOut)
            {
                return AttendanceErrors.AlreadyCheckedOut;
            }
            if (checkOutTime <= CheckInTime)
            {
                return AttendanceErrors.InvalidCheckOutTime;
            }
            CheckOutTime = checkOutTime;
            status = Status.CheckedOut;
            return Result.Updated;
        }
    }
}

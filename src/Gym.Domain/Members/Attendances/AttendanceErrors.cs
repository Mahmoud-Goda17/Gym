
using Gym.Domain.Results;

namespace Gym.Domain.Members.Attendances
{
    public static class AttendanceErrors
    {
        public static Error AlreadyCheckedOut =>
            Error.Conflict("Attendance.AlreadyCheckedOut", "The member has already checked out.");
        public static Error AlreadyCheckedIn =>
            Error.Conflict("Attendance.AlreadyCheckedIn", "The member has already checked in.");
        public static Error InvalidCheckOutTime =>
            Error.Validation("Attendance.InvalidCheckOutTime", "Check-out time must be after check-in time.");
        public static Error NoActiveCheckIn =>
            Error.Conflict("Attendance.NoActiveCheckIn", "The member does not have an active check-in.");
    }
}

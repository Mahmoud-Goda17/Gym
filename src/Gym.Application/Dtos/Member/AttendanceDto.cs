using Gym.Domain.Members.Attendances.Enums;
namespace Gym.Application.Dtos.Member
{
    public class AttendanceDto
    {
        public DateTimeOffset CheckInTime { get; set; }
        public DateTimeOffset? CheckOutTime { get; set; }
        public Status status { get; set; }

    }
}

using Gym.Application.Dtos.Member;
using Gym.Domain.Members.Attendances;

namespace Gym.Application.Mapper.Members
{
    public static class AttendanceMapper
    {
        public static AttendanceDto ToDto(this Attendance attendance)
        {
            ArgumentNullException.ThrowIfNull(attendance);
            return new AttendanceDto
            {
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                status = attendance.status,
            };
        }

        public static List<AttendanceDto> ToDtos(this List<Attendance> attendances)
        {
            return attendances.Select(a => a.ToDto()).ToList();
        }
    }
}

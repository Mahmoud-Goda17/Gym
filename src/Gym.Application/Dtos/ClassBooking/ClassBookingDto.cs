
using Gym.Domain.ClassBooking.Enums;

namespace Gym.Application.Dtos.ClassBooking
{
    public class ClassBookingDto
    {
        public Guid Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public BookingStatus Status { get; set; }
    }
}

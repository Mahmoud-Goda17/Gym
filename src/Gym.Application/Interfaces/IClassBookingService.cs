
using Gym.Application.Dtos.ClassBooking;
using Gym.Application.Dtos.Member;
using Gym.Domain.Results;

namespace Gym.Application.Interfaces
{
    public interface IClassBookingService
    {
        Task<Result<Guid>> BookClass(Guid memberId, Guid classId, CancellationToken ct);
        public  Task<Result<Success>> CancelBooking(Guid bookingId, CancellationToken ct);
        Task<Result<List<ClassBookingDto>>> GetMemberBookings(Guid memberId, CancellationToken ct);
        Task<Result<List<MemberDto>>> GetClassBookings(Guid classId, CancellationToken ct);
    }
}

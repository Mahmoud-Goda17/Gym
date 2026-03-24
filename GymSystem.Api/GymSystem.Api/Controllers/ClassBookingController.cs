using Gym.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ClassBookingController(IClassBookingService service) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> BookClass(Guid memberId, Guid classId, CancellationToken ct)
        {
            var result = await service.BookClass(memberId, classId, ct);
            return result.Match(
                bookingId => Ok(bookingId),
                errors => Problem(errors));
        }
        [HttpDelete("{bookingId:guid}")]
        public async Task<IActionResult> CancelBooking(Guid bookingId, CancellationToken ct)
        {
            var result = await service.CancelBooking(bookingId, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpGet("member/{memberId:guid}")]
        public async Task<IActionResult> GetMemberBookings(Guid memberId, CancellationToken ct)
        {
            var result = await service.GetMemberBookings(memberId, ct);
            return result.Match(
                bookings => Ok(bookings),
                errors => Problem(errors));
        }
        [HttpGet("class/{classId:guid}")]
        public async Task<IActionResult> GetClassBookings(Guid classId, CancellationToken ct)
        {
            var result = await service.GetClassBookings(classId, ct);
            return result.Match(
                members => Ok(members),
                errors => Problem(errors));
        }
    }
}

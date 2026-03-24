
using Gym.Application.Dtos.ClassBooking;
using Gym.Application.Dtos.Member;
using Gym.Application.Errors;
using Gym.Application.Interfaces;
using Gym.Domain.ClassBooking;
using Gym.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Gym.Application.Services
{
    public class ClassBookingService(
        IAppDbContext context,
        ILogger<ClassBookingService> logger
        ) : IClassBookingService
    {
        public async Task<Result<Guid>> BookClass(Guid memberId, Guid classId, CancellationToken ct)
        {
            var member = await context.Members
        .FirstOrDefaultAsync(m => m.Id == memberId, ct);

            if (member == null)
            {
                logger.LogWarning("Member {MemberId} not found.", memberId);
                return AplicationErrors.NotFound("Member", memberId);
            }

            var gymClass = await context.GymClasses
                .FirstOrDefaultAsync(c => c.Id == classId, ct);

            if (gymClass == null)
            {
                logger.LogWarning("GymClass {ClassId} not found.", classId);
                return AplicationErrors.NotFound("GymClass", classId);
            }

            if (gymClass.StartTime <= DateTimeOffset.UtcNow)
            {
                logger.LogWarning("GymClass {ClassId} is already started can not book it", classId);

                return ClassBookingErrors.ItsOverToBook;
            }
            
            var alreadyBooked = await context.ClassBooking
                .AnyAsync(b => b.MemberId == memberId && b.ClassId == classId, ct);

            if (alreadyBooked)
            {
                logger.LogWarning(
                    "Member {MemberId} Has Already booked class {ClassId}.",
                    memberId, classId);

                return ClassBookingErrors.MemberAlreadybooked;

            }

            var hasConflict = await context.ClassBooking
                     .Where(b => b.MemberId == memberId)
                     .AnyAsync(b =>
                         b.gymClass.StartTime < gymClass.EndTime &&
                         b.gymClass.EndTime > gymClass.StartTime, ct);

            if (hasConflict)
            {
                logger.LogWarning(
                    "Member {MemberId} Has Already booked Another class {ClassId} at the Same Time.",
                    memberId, classId);

                return ClassBookingErrors.MemberBusy;
            }

            var increasebooking = gymClass.IncreaseBooking();
            if(increasebooking.IsError)
            {
                logger.LogWarning("GymClass {ClassId} is Full can not book any more", classId);

                return increasebooking.Errors;
            }
            var booking = ClassBooking.Create(
                Guid.NewGuid(),
                memberId, classId);
            if (booking.IsError)
            {
                logger.LogWarning("booking creation failed: {Errors}", booking.Errors);

                return booking.Errors;
            }
            context.ClassBooking.Add(booking.Value);
            await context.SaveChangesAsync(ct);

            logger.LogInformation(
                "Member {MemberId} booked class {ClassId}.",
                memberId, classId);

            return booking.Value.Id;
        }

        public async Task<Result<Success>> CancelBooking(Guid bookingId, CancellationToken ct)
        {
             var booking = await context.ClassBooking
                 .Include(b => b.gymClass)
                 .FirstOrDefaultAsync(b => b.Id == bookingId, ct);

            if (booking == null)
            {
                logger.LogWarning("ClassBooking {bookingId} not found.", bookingId);

                return AplicationErrors.NotFound("Booking", bookingId);
            }

            if (booking.gymClass.StartTime <= DateTimeOffset.UtcNow)
            {
                logger.LogWarning("GymClass {ClassId} is already started can not book it", booking.ClassId);

                return ClassBookingErrors.ItsOverToBook;
            }

            var decrease = booking.gymClass.DecreasingBooking();
            if(decrease.IsError)
            {
                logger.LogWarning("GymClass {ClassId} is Empty can not Decrease any more", booking.ClassId);

                return decrease.Errors;
            }

            var cancled = booking.Canceld();
            if (cancled.IsError)
            {
                logger.LogWarning("booking cancled failed: {Errors}", cancled.Errors);
            }
            await context.SaveChangesAsync(ct);
            logger.LogInformation("Booking {BookingId} cancelled.", bookingId);
            return Result.Success;
        }

        public async Task<Result<List<MemberDto>>> GetClassBookings(Guid classId, CancellationToken ct)
        {
            var members = await context.ClassBooking
                         .Where(b => b.ClassId == classId)
                         .Select(b => new MemberDto
                         {
                                 Id = b.member!.Id,
                                 FullName=b.member.FullName!,
                                 Phone=b.member.Phone!,
                                 Email=b.member.Email!,
                                 type = b.member.memberShips
                                         .OrderBy(ms => ms.StartDate)
                                         .Select(ms => ms.Type)
                                         .LastOrDefault()
                         }) 
                         .ToListAsync(ct);

            return members;
        }

        public async Task<Result<List<ClassBookingDto>>> GetMemberBookings(Guid memberId, CancellationToken ct)
        {
            var bookings = await context.ClassBooking
                      .AsNoTracking()
                      .Where(b => b.MemberId == memberId)
                      .Select(b => new ClassBookingDto
                      {
                         Id = b.Id,
                         ClassName = b.gymClass.Name,
                         StartTime = b.gymClass.StartTime,
                         EndTime = b.gymClass.EndTime,
                         Status = b.Status
                      })
                       .ToListAsync(ct);
 
          return bookings;
        }
    }
}

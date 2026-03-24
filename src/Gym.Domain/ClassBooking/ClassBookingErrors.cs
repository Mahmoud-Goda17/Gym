
using Gym.Domain.Results;

namespace Gym.Domain.ClassBooking
{
    public static class ClassBookingErrors
    {
        public static Error MemberIdRequired =>
            Error.Validation("ClassBooking.MemberIdRequired", "Member ID is required.");
        public static Error ClassIdRequired =>
            Error.Validation("ClassBooking.ClassIdRequired", "Class ID is required.");
        public static Error AlreadyCancelled =>
            Error.Conflict("ClassBooking.AlreadyCancelled", "The booking is already cancelled.");
        public static Error ItsOverToBook =>
            Error.Validation("ClassBooking.ItsOverToBook", "Cannot book past class.");
        public static Error MemberAlreadybooked =>
            Error.Validation("ClassBooking.MemberAlreadybooked", "Member already booked this class.");
        public static Error MemberBusy =>
            Error.Validation("ClassBooking.MemberBusy", "Member has another class at same time");
    }
}

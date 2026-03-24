
using Gym.Domain.Results;

namespace Gym.Domain.GymClasss
{
    public static class GymClassErrors
    {
        public static Error NameRequired =>
            Error.Validation("GymClass.Name.Required", "Class name is required.");
        public static Error InvalidTimeRange =>
            Error.Validation("GymClass.InvalidTimeRange", "End time must be after start time.");
        public static Error InvalidCapacity =>
            Error.Validation("GymClass.InvalidCapacity", "Capacity must be greater than zero.");
        public static Error ClassFull =>
            Error.Conflict("GymClass.ClassFull", "The class is fully booked.");
        public static Error NoBookingsToCancel =>
            Error.Conflict("GymClass.NoBookingsToCancel", "There are no bookings to cancel.");
        public static Error NotAvalibleTrainer =>
            Error.Validation("GymClass.NotAvalibleTrainer", "The trainer is not available at the specified time.");
    }
}

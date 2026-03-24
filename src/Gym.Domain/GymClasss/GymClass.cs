
using Gym.Domain.Common;
using Gym.Domain.Results;

namespace Gym.Domain.GymClasss
{
    public sealed class GymClass : AuditableEntity
    {
        public string Name { get; private set; }
        public Guid TrainerId { get; private set; }

        public DateTimeOffset StartTime { get; private set; }
        public DateTimeOffset EndTime { get; private set; }

        public int Capacity { get; private set; }
        public int BookedCount { get; private set; }

        private GymClass() { }
        private GymClass(Guid id, string name, Guid trainerId, DateTimeOffset startTime, DateTimeOffset endTime, int capacity)
            : base(id)
        {
            Name = name;
            TrainerId = trainerId;
            StartTime = startTime;
            EndTime = endTime;
            Capacity = capacity;
            BookedCount = 0;
        }
        public static Result<GymClass> Created(Guid id, string name, Guid trainerId, DateTimeOffset startTime, DateTimeOffset endTime, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GymClassErrors.NameRequired;
            }
            if (endTime <= startTime)
            {
                return GymClassErrors.InvalidTimeRange;
            }
            if (capacity <= 0)
            {
                return GymClassErrors.InvalidCapacity;
            }
            return new GymClass(id, name, trainerId, startTime, endTime, capacity);
        }
        public Result<Updated> Update(string name, Guid trainerId, DateTimeOffset startTime, DateTimeOffset endTime, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GymClassErrors.NameRequired;
            }
            if (endTime <= startTime)
            {
                return GymClassErrors.InvalidTimeRange;
            }
            if (capacity <= 0)
            {
                return GymClassErrors.InvalidCapacity;
            }
            Name = name;
            TrainerId = trainerId;
            StartTime = startTime;
            EndTime = endTime;
            Capacity = capacity;
            return Result.Updated;
        }
        public Result<Updated> IncreaseBooking()
        {
            if (BookedCount >= Capacity)
            {
                return GymClassErrors.ClassFull;
            }
            BookedCount++;
            return Result.Updated;
        }
        public Result<Updated> DecreasingBooking()
        {
            if (BookedCount <= 0)
            {
                return GymClassErrors.NoBookingsToCancel;
            }
            BookedCount--;
            return Result.Updated;
        }
    }
}

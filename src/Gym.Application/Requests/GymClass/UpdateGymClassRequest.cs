
namespace Gym.Application.Requests.GymClass
{
    public sealed record UpdateGymClassRequest(
        Guid Id,
        string Name,
        Guid TrainerId,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        int Capacity)
    {
    }
}

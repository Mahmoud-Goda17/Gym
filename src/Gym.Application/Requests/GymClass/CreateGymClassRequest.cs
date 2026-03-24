
namespace Gym.Application.Requests.GymClass
{
    public sealed record CreateGymClassRequest(
        string Name,
        Guid TrainerId,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime,
        int Capacity)
    {
    }
}

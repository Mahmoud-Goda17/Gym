
namespace Gym.Application.Requests.Trainer
{
    public sealed record UpdateTrainerRequest(
        Guid Id,
        string FullName,
        string Email,
        string Phone,
        decimal HourlyRate)
    {
    }
}

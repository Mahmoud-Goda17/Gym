
namespace Gym.Application.Requests.Trainer
{
    public sealed record CreateTrainerRequest(
        string FullName,
        string Email,
        string Phone,
        decimal HourlyRate
    )
    {
    }
}


using Gym.Application.Dtos.Trainer;
using Gym.Application.Requests.Trainer;
using Gym.Domain.Results;

namespace Gym.Application.Interfaces
{
    public interface ITrainerService
    {
        Task<Result<TrainerDto>> CreateTrainer(CreateTrainerRequest request, CancellationToken ct);
        Task<Result<Updated>> UpdateTrainer(UpdateTrainerRequest request, CancellationToken ct);
        Task<Result<Deleted>> DeleteTrainer(Guid id, CancellationToken ct); 
        Task<Result<TrainerDto>> GetTrainerById(Guid id, CancellationToken ct);
        Task<Result<List<TrainerDto>>> GetAllTrainers(CancellationToken ct);
        Task<Result<List<TrainerDto>>> GetActiveTrainers(CancellationToken ct);
        Task<Result<Updated>> Activetrainer (Guid id,CancellationToken ct);
        Task<Result<List<TrainerDto>>> SearchTrainer(string name, CancellationToken ct);

    }
}

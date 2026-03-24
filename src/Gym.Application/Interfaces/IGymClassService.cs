
using Gym.Application.Dtos.GymClass;
using Gym.Application.Requests.GymClass;
using Gym.Domain.Results;

namespace Gym.Application.Interfaces
{
    public interface IGymClassService
    {
        Task<Result<GymClassDto>> CreateGymClass(CreateGymClassRequest request, CancellationToken ct);
        Task<Result<Updated>> UpdateGymClasses(UpdateGymClassRequest request, CancellationToken ct);
        Task<Result<Deleted>> DeleteGymClass(Guid id, CancellationToken ct);
        Task<Result<GymClassDto>> GetGymClassById(Guid id, CancellationToken ct);
        Task<Result<List<GymClassDto>>> GetAllGymClasses(CancellationToken ct);
        Task<Result<List<GymClassDto>>> GetUpCommingGymClasses(CancellationToken ct);
        Task<Result<List<GymClassDto>>> GetGymClassesByTrainerId(Guid trainerId, CancellationToken ct);
        Task<Result<decimal>> CalculateTrainerIncome(
            Guid trainerId,
            DateTimeOffset? from,
            DateTimeOffset? to,
            CancellationToken ct);

    }

}

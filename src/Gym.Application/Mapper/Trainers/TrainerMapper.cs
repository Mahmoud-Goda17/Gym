using Gym.Application.Dtos.Trainer;
using Gym.Domain.Trainers;
namespace Gym.Application.Mapper.Trainers
{
    public static class TrainerMapper
    {
        public static TrainerDto ToDto(this Trainer trainer)
        {
            ArgumentNullException.ThrowIfNull(trainer);
            return new TrainerDto
            {
                Id = trainer.Id,
                FullName = trainer.FullName!,
                Email = trainer.Email!,
                Phone = trainer.Phone!,
                HourlyRate = trainer.HourlyRate,
                IsActive = trainer.IsActive
            };
        }
        public static List<TrainerDto> ToDtos(this IEnumerable<Trainer> trainers)
        {
            return trainers.Select(t => t.ToDto()).ToList();
        }
    }
}

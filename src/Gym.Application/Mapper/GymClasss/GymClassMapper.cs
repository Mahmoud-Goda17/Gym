using Gym.Application.Dtos.GymClass;
using Gym.Domain.GymClasss;
using Gym.Domain.Members.Attendances;

namespace Gym.Application.Mapper.GymClasss
{
    public static class GymClassMapper
    {
        public static GymClassDto ToDto(this GymClass gymClass)
        {
            ArgumentNullException.ThrowIfNull(gymClass);

            return new GymClassDto
            {
                Id = gymClass.Id,
                Name = gymClass.Name,
                TrainerId = gymClass.TrainerId,
                StartTime = gymClass.StartTime,
                EndTime = gymClass.EndTime,
                Availables = gymClass.Capacity - gymClass.BookedCount
            };
        }
        public static List<GymClassDto> ToDtos(this IEnumerable<GymClass> gymClasses)
        {
            return gymClasses.Select(gc => gc.ToDto()).ToList();
        }           
    }
}

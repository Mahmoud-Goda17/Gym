namespace Gym.Application.Dtos.GymClass
{
    public class GymClassDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid TrainerId { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int Availables { get; set; } 
    }

}

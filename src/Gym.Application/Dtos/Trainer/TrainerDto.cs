
namespace Gym.Application.Dtos.Trainer
{
    public class TrainerDto
    {
        public Guid Id { get; set; }
        public string FullName { get;  set; }=string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }

    }
}

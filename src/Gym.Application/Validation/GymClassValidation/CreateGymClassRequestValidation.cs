
using FluentValidation;
using Gym.Application.Requests.GymClass;

namespace Gym.Application.Validation.GymClassValidation
{
    public class CreateGymClassRequestValidation : AbstractValidator<CreateGymClassRequest>
    {
        public CreateGymClassRequestValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
            RuleFor(x => x.TrainerId)
                .NotEmpty().WithMessage("TrainerId is required.");
            RuleFor(x => x.StartTime)
                .GreaterThan(DateTimeOffset.UtcNow).WithMessage("StartTime must be in the future.");
            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime).WithMessage("EndTime must be after StartTime.");
            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0.");
        }
    }
}

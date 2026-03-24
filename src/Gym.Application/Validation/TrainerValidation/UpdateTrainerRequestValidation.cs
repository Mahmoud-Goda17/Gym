
using FluentValidation;
using Gym.Application.Requests.Trainer;

namespace Gym.Application.Validation.TrainerValidation
{
    public class UpdateTrainerRequestValidation: AbstractValidator<UpdateTrainerRequest>
    {
        public UpdateTrainerRequestValidation()
        { 
            RuleFor(x => x.Id).NotEmpty()
                .WithMessage("Trainer ID is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.").
                MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email).
                NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Phone).
                NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.HourlyRate)
                .GreaterThan(0).WithMessage("Hourly rate must be greater than zero."); }
    }
}

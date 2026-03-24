
using FluentValidation;
using Gym.Application.Requests.Aurh;

namespace Gym.Application.Validation.Auth
{
    public class LogInValidator : AbstractValidator<LogIn>
    {
        public LogInValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters");
        }
    }
}

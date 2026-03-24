
using FluentValidation;
using Gym.Application.Requests.Member;

namespace Gym.Application.Validation.MemberValidation
{
    public class CreateMemberRequestValidation : AbstractValidator<CreateMemberRequest>
    {
        public CreateMemberRequestValidation()
        {
            RuleFor(x => x.fullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100);
            RuleFor(x => x.email)
                .EmailAddress().WithMessage("Invalid email")
                .MaximumLength(100);

            RuleFor(x => x.phone)
             .NotEmpty().WithMessage("Phone number is required.")
             .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must be 7–15 digits and may start with '+'.");

            RuleFor(x => x.startDate)
                .GreaterThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("StartDate must be in the future.");
            RuleFor(x => x.type)
                .IsInEnum().WithMessage("Invalid membership type.");
        }
    }
}

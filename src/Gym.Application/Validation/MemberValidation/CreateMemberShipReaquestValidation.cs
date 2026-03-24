
using FluentValidation;
using Gym.Application.Requests.Member;

namespace Gym.Application.Validation.MemberValidation
{
    public class CreateMemberShipReaquestValidation:AbstractValidator<CreateMemberShipReaquest>
    {
        public CreateMemberShipReaquestValidation()
        {

           RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("StartDate must be in the future.");
            RuleFor(x => x.MemberId)
                .NotEmpty().WithMessage("Member ID is required.");
            RuleFor(x=>x.Type)
                .IsInEnum().WithMessage("Invalid membership type.");

        }
    }
}

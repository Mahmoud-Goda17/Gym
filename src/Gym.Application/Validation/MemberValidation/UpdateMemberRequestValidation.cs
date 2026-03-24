
using FluentValidation;
using Gym.Application.Requests.Member;

namespace Gym.Application.Validation.MemberValidation
{
    public class UpdateMemberRequestValidation:AbstractValidator<UpdateMemberReaquest>
    {
        public UpdateMemberRequestValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Member ID is required.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Invalid phone number format.");
        }
    }
}

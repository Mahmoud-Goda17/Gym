
using FluentValidation;
using Gym.Application.Requests.Aurh;

namespace Gym.Application.Validation.Auth
{
    public  class AddRoleModelValidator : AbstractValidator<AddRoleModel>
    {
        public AddRoleModelValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role is required");
        }
    }
}

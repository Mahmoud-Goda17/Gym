
using Gym.Domain.Results;

namespace Gym.Domain.Trainers
{
    public static class TrainerErrors
    {
        public static Error FullNameRequired =>
            Error.Validation("Trainer.FullName.Required", "Full name is required.");
        public static Error EmailRequired =>
            Error.Validation("Trainer.Email.Required", "Email is required.");
        public static Error EmailInvalid =>
            Error.Validation("Trainer.Email.Invalid", "Email is invalid.");
        public static Error InvalidPhoneNumber =>
            Error.Validation("Trainer.InvalidPhoneNumber", "Phone number must be 7–15 digits and may start with '+'.");
        public static Error InvalidHourlyRate =>
            Error.Validation("Trainer.InvalidHourlyRate", "Hourly rate must be greater than zero.");
        public static Error TrainerInactive =>
            Error.Validation("Trainer.Inactive", "can not do nothing in  an inactive trainer.");
        public static Error TrainerAlreadyInactive =>
            Error.Validation("Trainer.AlreadyInactive", "Trainer is already inactive.");
        public static Error TrainerAlreadyActive =>
            Error.Validation("Trainer.AlreadyActive", "Trainer is already active.");
        public static Error TrainerExists =>
                   Error.Conflict("Trainer.Exists", "A Trainer with the same email or the same phone number already exists.");

    }
}

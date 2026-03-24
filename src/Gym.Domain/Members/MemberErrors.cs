
using Gym.Domain.Results;

namespace Gym.Domain.Members
{
    public static  class MemberErrors
    {
        public static Error FullNameRequired =>
            Error.Validation("Member.FullName.Required", "Full name is required.");
        public static Error PhoneNumberRequired =>
             Error.Validation("Member_Number_Required", "Phone number is required");

        public static Error EmailRequired =>
            Error.Validation("Member_Email_Required", "Email is required");

        public static Error EmailInvalid =>
            Error.Validation("Member_Email_Invalid", "Email is invalid");

        public static  Error InvalidPhoneNumber =>
            Error.Conflict("Member.InvalidPhoneNumber", "Phone number must be 7–15 digits and may start with '+'.");

        public static Error InactiveMember=>
            Error.Validation("Member.Inactive", "Cannot add membership to an inactive member.");
        public static Error MemberExists =>
            Error.Conflict("Member.Exists", "A member with the same email or the same phone number already exists.");
        public static Error AlreadyInactive =>
            Error.Validation("Member.AlreadyInactive", "Member is already inactive.");
        public static Error AlreadyActive =>
            Error.Validation("Member.AlreadyActive", "Member is already active.");



    }
}

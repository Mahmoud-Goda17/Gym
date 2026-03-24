
using Gym.Domain.Results;

namespace Gym.Domain.Members.Memberships
{
    public static class MemberShipErrors
    {
        public static Error InvalidTimming =>
            Error.Conflict("Membership.InvalidTiming", "End time must be after start time");
        public static Error InvalidMembershipType =>
            Error.Validation("Membership.InvalidType", "Membership type is invalid.");
        public static Error InvalidMembershipStatus =>
            Error.Validation("Membership.InvalidStatus", "Membership status is invalid.");

        public static Error DuplicateActiveMembership =>
            Error.Conflict("Membership.DuplicateActiveMembership", "An active membership of this type already exists for the member.");
        public static Error NoActiveMembership =>
            Error.Conflict("Membership.NoActiveMembership", "The member does not have an active membership for the specified time.");
        public static Error AlreadyFrozen =>
            Error.Conflict("Membership.AlreadyFrozen", "Membership is already frozen");
        public static Error AlreadyNotFrozen =>
            Error.Conflict("Membership.AlreadyNotFrozen", "Membership is already Not frozen");
    }
}

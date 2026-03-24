using Gym.Domain.Members.Memberships.Enums;

namespace Gym.Application.Requests.Member
{
    public sealed record CreateMemberShipReaquest(
        Guid MemberId,
        MembershipType Type,
        DateTimeOffset StartDate)
    {
    }
}

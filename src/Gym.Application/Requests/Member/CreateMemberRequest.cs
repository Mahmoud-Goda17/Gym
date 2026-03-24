using Gym.Domain.Members.Memberships.Enums;

namespace Gym.Application.Requests.Member
{
    public sealed record CreateMemberRequest(
        string fullName,
        string email,
        string phone,
        MembershipType type,
        DateTimeOffset startDate);
}

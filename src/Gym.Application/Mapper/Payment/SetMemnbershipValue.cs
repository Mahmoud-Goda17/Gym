
using Gym.Domain.Members.Memberships.Enums;

namespace Gym.Application.Mapper.Payment
{
    public static class SetMemnbershipValue
    {
        public static decimal GetMembershipValue(MembershipType type)
        {
            return type switch
            {
               MembershipType.Monthly => 500,
               MembershipType.Quarterly => 1400,
               MembershipType.Yearly => 5000,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Not expected membership type value: {type}"),
            };
        }
    }
}

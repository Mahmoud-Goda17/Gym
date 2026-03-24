using Gym.Domain.Members.Memberships.Enums;

namespace Gym.Application.Dtos.Member
{
    public class MembershipDto
    {
        public MembershipType Type { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public MembershipStatus Status { get; set; }
        public bool IsFrozen { get; set; }

    }
}

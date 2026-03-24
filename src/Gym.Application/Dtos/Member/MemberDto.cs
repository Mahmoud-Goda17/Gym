using Gym.Domain.Members.Memberships.Enums;

namespace Gym.Application.Dtos.Member
{
    public class MemberDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public MembershipType? type { get; set; }
    }
}

using Gym.Application.Dtos.Member;
using Gym.Domain.Members.Memberships;
namespace Gym.Application.Mapper.Members
{
    public static class MembershipMapper
    {
        public static MembershipDto ToDto(this MemberShip memberShip)
        {
            ArgumentNullException.ThrowIfNull(memberShip);
            return new MembershipDto
            {
                Type = memberShip.Type,
                EndDate = memberShip.EndDate,
                IsFrozen = memberShip.IsFrozen,
                StartDate = memberShip.StartDate,
                Status = memberShip.Status,
            };

        }
        public static List<MembershipDto> ToDtos(this List<MemberShip> memberShips)
        {
            return memberShips.Select(x => x.ToDto()).ToList();
        }
    }
}

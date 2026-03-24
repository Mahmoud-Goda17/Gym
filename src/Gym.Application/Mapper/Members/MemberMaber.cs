using Gym.Application.Dtos.Member;
using Gym.Domain.Members;

namespace Gym.Application.Mapper.Members
{
    public static class MemberMaber
    {
        public static MemberDto ToDto(this Member member)
        {
            ArgumentNullException.ThrowIfNull(member);

            return new MemberDto
            {
                Id = member.Id,
                FullName = member.FullName!,
                Email = member.Email!,
                Phone = member.Phone!,
                type = member.memberShips?.OrderBy(m => m.StartDate).LastOrDefault()?.Type
            };
        }
        public static List<MemberDto> ToDtos(this IEnumerable<Member> members)
        {
            return members.Select(m => m.ToDto()).ToList();
        }
    }
}

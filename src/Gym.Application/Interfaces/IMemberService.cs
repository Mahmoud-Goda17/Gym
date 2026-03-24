using Gym.Application.Dtos.Member;
using Gym.Application.Requests.Member;
using Gym.Domain.Results;

namespace Gym.Application.Interfaces
{
    public interface IMemberService
    {
        Task<Result<MemberDto>> CreateMember(CreateMemberRequest member,CancellationToken ct);
        Task<Result<Updated>> UpdateMember (UpdateMemberReaquest member, CancellationToken ct);
        Task<Result<Deleted>> DeleteMember(Guid id, CancellationToken ct);
        Task<Result<MemberDto>> GetMemberById(Guid id, CancellationToken ct);
        Task<Result<List<MemberDto>>> GetAllMembers(CancellationToken ct);
        Task<Result<List<MemberDto>>> GetActiveMembers(CancellationToken ct);
        Task<Result<Success>> AddMemberShip (CreateMemberShipReaquest request, CancellationToken ct);
        Task<Result<Success>> ChickIn(Guid id,CancellationToken ct);
        Task<Result<Success>> ChickOut(Guid id, CancellationToken ct);
        Task<Result<Updated>> ActivateMember(Guid id, CancellationToken ct);
        Task<Result<List<AttendanceDto>>> GetAttendances(Guid memberId, CancellationToken ct);
        Task<Result<List<MembershipDto>>> GetMemberships(Guid memberId, CancellationToken ct);
        Task<Result<Updated>> FreezMembership(Guid memberid,Guid memberShipId, CancellationToken ct);
        Task<Result<Updated>> DeFreezMembership(Guid memberid, Guid memberShipId, CancellationToken ct);
        Task<Result<AttendanceDto>> GetLastAttendances(Guid memberId, CancellationToken ct);
        Task<Result<List<MemberDto>>> SearchMembers(string name, CancellationToken ct);




    }
}

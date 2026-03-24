using Gym.Application.Dtos.Member;
using Gym.Application.Errors;
using Gym.Application.Interfaces;
using Gym.Application.Mapper.Members;
using Gym.Application.Requests.Member;
using Gym.Domain.Members;
using Gym.Domain.Members.Memberships;
using Gym.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
namespace Gym.Application.Services
{
    public class MemberService(
        ILogger<MemberService> logger,
        IAppDbContext context,
        IMemoryCache cache) : IMemberService
    {
        public async Task<Result<MemberDto>> CreateMember(CreateMemberRequest member, CancellationToken ct)
        {
            var email = member.email.Trim().ToLower();
            var phone = member.phone.Trim();

            var exists = await context.Members.AnyAsync(
                c => c.Email!.ToLower() == email || c.Phone == phone,
                ct);

            if (exists)
            {
                logger.LogWarning("Member creation aborted. Email or Phone already exists.");

                return MemberErrors.MemberExists;
            }
            var newMemberResult = Member.Create(
                Guid.NewGuid(),
                member.fullName.Trim(),
                member.email.Trim(),
                member.phone.Trim());
            if (newMemberResult.IsError)
            {
                logger.LogWarning("Member creation failed. Validation errors: {Errors}", newMemberResult.Errors);
                return newMemberResult.Errors;
            }

            var newMember = newMemberResult.Value;
            var memership = newMember.AddMemberShip(member.type, member.startDate);
            if (memership.IsError)
            {
                logger.LogWarning("Membership creation failed. Validation errors: {Errors}", memership.Errors);
                return memership.Errors;
            }
            context.Members.Add(newMember);
            await context.SaveChangesAsync(ct);
            ClearMemberCache();
            logger.LogInformation("Member created successfully with ID: {MemberId}", newMember.Id);
            return newMember.ToDto();
        }
        public async Task<Result<Updated>> UpdateMember(UpdateMemberReaquest member, CancellationToken ct)
        {
            var email = member.Email.Trim().ToLower();
            var phone = member.PhoneNumber.Trim();

            var exists = await context.Members.AnyAsync(c =>
                    c.Id != member.Id &&
                    (
                        c.Email!.ToLower() == email ||
                        c.Phone == phone
                    ),
                    ct);
            if (exists)
            {
                logger.LogWarning(
                    "Member update aborted. Email or Phone already exists for another Member.");
                   
                return MemberErrors.MemberExists;
            }
            var existingMember = await context.Members.FirstOrDefaultAsync(m => m.Id == member.Id, ct);
            if (existingMember == null)
            {
                logger.LogWarning("Member update failed. Member not found. MemberId: {MemberId}", member.Id);
                return AplicationErrors.NotFound("Member", member.Id);
            }
            var updatemember = existingMember.Update(member.Name, member.Email, member.PhoneNumber);
            if (updatemember.IsError)
            {
                return updatemember.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearMemberCache();
            logger.LogInformation("Member updated successfully. MemberId: {MemberId}", member.Id);
            return Result.Updated;
        }
        public async Task<Result<Deleted>> DeleteMember(Guid id, CancellationToken ct)
        {
            var existingMember = await context.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", id);
                return AplicationErrors.NotFound("Member", id);
            }
            var deactivatemember = existingMember.DeActive();
            if (deactivatemember.IsError)
            {
                return deactivatemember.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearMemberCache();
            logger.LogInformation("Member deactivated successfully. MemberId: {MemberId}", id);
            return Result.Deleted;

        }

        public async Task<Result<MemberDto>> GetMemberById(Guid id, CancellationToken ct)
        {
            var member = await context.Members.AsNoTracking()
                    .Where(m => m.Id == id)
                    .Select(m => new MemberDto
                    {
                        Id = m.Id,
                        FullName = m.FullName!,
                        Email = m.Email!,
                        Phone = m.Phone!,
                        type = m.memberShips
                            .OrderBy(ms => ms.StartDate)
                            .Select(ms => ms.Type)
                            .LastOrDefault()
                    })
                    .FirstOrDefaultAsync(ct);
            if (member == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}",id);
                return AplicationErrors.NotFound("Member", id);
            }
            logger.LogInformation("Retrieved Member with ID {MemberId} successfully.", id);

            return member;

        }

        public async Task<Result<List<MemberDto>>> GetAllMembers(CancellationToken ct)
        {
            var cacheKey = "members";

            var members = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.Size = 1;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                var membersResponse = await context.Members
                    .AsNoTracking()
                    .Select(m => new MemberDto
                    {
                        Id = m.Id,
                        FullName = m.FullName!,
                        Email = m.Email!,
                        Phone = m.Phone!,
                        type = m.memberShips
                            .OrderBy(ms => ms.StartDate)
                            .Select(ms => ms.Type)
                            .LastOrDefault()
                    })
                    .ToListAsync(ct);

                return membersResponse;
            });

            return members;
        }
        public async Task<Result<List<MemberDto>>> GetActiveMembers(CancellationToken ct)
        {
            var cacheKey = "activemembers";

            var members = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.Size = 1;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                var membersResponse = await context.Members
                    .Where(m => m.IsActive)
                    .AsNoTracking()
                    .Select(m => new MemberDto
                    {
                        Id = m.Id,
                        FullName = m.FullName!,
                        Email = m.Email!,
                        Phone = m.Phone!,
                        type = m.memberShips
                            .OrderBy(ms => ms.StartDate)
                            .Select(ms => ms.Type)
                            .LastOrDefault()
                    })
                    .ToListAsync(ct);

                return membersResponse;
            });
            logger.LogInformation("Retrieved active member from cache or database. Count: {Count}", members.Count);

            return members;

        }

        public async Task<Result<Success>> AddMemberShip(CreateMemberShipReaquest request, CancellationToken ct)
        {
            var existingMember = await context.Members.FirstOrDefaultAsync(m => m.Id == request.MemberId, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", request.MemberId);
                return AplicationErrors.NotFound("Member", request.MemberId);
            }
            var memership = existingMember.AddMemberShip(request.Type,request.StartDate);
            if (memership.IsError)
            {
                logger.LogWarning("Membership creation failed. Validation errors: {Errors}", memership.Errors);
                return memership.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearMemberCache();
            logger.LogInformation("Membership added successfully for MemberId: {MemberId}", request.MemberId);
            return Result.Success;
        }

        public async Task<Result<Success>> ChickIn(Guid id, CancellationToken ct)
        {
            var existingMember = await context.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", id);
                return AplicationErrors.NotFound("Member", id);
            }
            var checkin = existingMember.CheckIn(DateTimeOffset.UtcNow);
            if (checkin.IsError)
            {
                logger.LogWarning("Check-in failed. Validation errors: {Errors}", checkin.Errors);
                return checkin.Errors;
            }
            await context.SaveChangesAsync(ct);
            logger.LogInformation("Member checked in successfully. MemberId: {MemberId}", id);
            return Result.Success;
        }

        public async Task<Result<Success>> ChickOut(Guid id, CancellationToken ct)
        {
            var existingMember = await context.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", id);
                return AplicationErrors.NotFound("Member", id);
            }
            var checkout = existingMember.CheckOut(DateTimeOffset.UtcNow);
            if (checkout.IsError)
            {
                logger.LogWarning("Check-out failed. Validation errors: {Errors}", checkout.Errors);
                return checkout.Errors;
            }
            await context.SaveChangesAsync(ct);
            logger.LogInformation("Member checked out successfully. MemberId: {MemberId}", id);
            return Result.Success;
        }

        public async Task<Result<Updated>> ActivateMember(Guid id, CancellationToken ct)
        {
            var existingMember = await context.Members.FirstOrDefaultAsync(m => m.Id == id, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", id);
                return AplicationErrors.NotFound("Member", id);
            }
            var activatemember = existingMember.Active();
            if (activatemember.IsError)
            {
                logger.LogWarning("Member activation failed. Validation errors: {Errors}", activatemember.Errors);
                return activatemember.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearMemberCache();
            logger.LogInformation("Member activated successfully. MemberId: {MemberId}", id);
            return Result.Updated;

        }

        public async Task<Result<List<AttendanceDto>>> GetAttendances(Guid memberId, CancellationToken ct)
        {
        {
            var memberExists = await context.Members
                .AsNoTracking()
                .AnyAsync(m => m.Id == memberId, ct);

            if (!memberExists)
            {
                logger.LogWarning("Member not found. MemberId: {MemberId}", memberId);
                return AplicationErrors.NotFound("Member", memberId);
            }

            var attendances = await context.Attendances
                .AsNoTracking()
                .Where(a => a.MeberId == memberId)
                .OrderByDescending(a => a.CheckInTime)
                .Select(a => new AttendanceDto
                {
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    status = a.status

                })
                .ToListAsync(ct);

            if (attendances.Count == 0)
            {
                logger.LogWarning("Member With Id:{MemberId} does not have attendances", memberId);
                return AplicationErrors.NotFound("Attendance", memberId);
            }

            return attendances;
        }
        }

        public async Task<Result<List<MembershipDto>>> GetMemberships(Guid memberId, CancellationToken ct)
        {
            var existingMember = await context.Members.Include(M => M.memberShips)
                              .AsNoTracking()
                              .FirstOrDefaultAsync(m => m.Id == memberId, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", memberId);
                return AplicationErrors.NotFound("Member", memberId);
            }
            List<MemberShip> memberShips = [];
            foreach (MemberShip memberShip in existingMember.memberShips)
            {
                memberShips.Add(memberShip);
            }
            if (memberShips.Count == 0)
            {
                logger.LogWarning("Member With Id:{MemberId} not Have Any MemberShips Yet", memberId);
                return AplicationErrors.NotFound("MemberShip", memberId);
            }
            return memberShips.ToDtos();
        }

        public async Task<Result<Updated>> FreezMembership(Guid memberid, Guid memberShipId, CancellationToken ct)
        {
            var existingMember = await context.Members.Include(M => M.memberShips)
                  .FirstOrDefaultAsync(m => m.Id == memberid, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", memberid);
                return AplicationErrors.NotFound("Member", memberid);
            }
            var freezMembership = existingMember.FrozenMempership(memberShipId);
            if (freezMembership.IsError)
            {
                logger.LogWarning("Membership freezing failed. Validation errors: {Errors}", freezMembership.Errors);
                return freezMembership.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearMemberCache();
            logger.LogInformation("Membership frozen successfully. MemberId: {MemberId}, MembershipId: {MembershipId}", memberid, memberShipId);
            return Result.Updated;

        }

        public async Task<Result<Updated>> DeFreezMembership(Guid memberid, Guid memberShipId, CancellationToken ct)
        {
            var existingMember = await context.Members.Include(M => M.memberShips)
                                  .FirstOrDefaultAsync(m => m.Id == memberid, ct);
            if (existingMember == null)
            {
                logger.LogWarning(" Member not found. MemberId: {MemberId}", memberid);
                return AplicationErrors.NotFound("Member", memberid);
            }
            var deFreezMembership = existingMember.DeFrozenMempership(memberShipId);
            if (deFreezMembership.IsError)
            {
                logger.LogWarning("Membership de-freezing failed. Validation errors: {Errors}", deFreezMembership.Errors);
                return deFreezMembership.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearMemberCache();
            logger.LogInformation("Membership de-frozen successfully. MemberId: {MemberId}, MembershipId: {MembershipId}", memberid, memberShipId);
            return Result.Updated;
        }

        public async Task<Result<AttendanceDto>> GetLastAttendances(Guid memberId, CancellationToken ct)
        {
        {
            var memberExists = await context.Members
                .AsNoTracking()
                .AnyAsync(m => m.Id == memberId, ct);

            if (!memberExists)
            {
                logger.LogWarning("Member not found. MemberId: {MemberId}", memberId);
                return AplicationErrors.NotFound("Member", memberId);
            }

            var lastAttendance = await context.Attendances
                .AsNoTracking()
                .Where(a => a.MeberId == memberId)
                .OrderByDescending(a => a.CheckInTime)
                .Select(a => new AttendanceDto
                {
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    status = a.status
                })
                .FirstOrDefaultAsync(ct);

            if (lastAttendance == null)
            {
                logger.LogWarning("Member With Id:{MemberId} does not have attendances", memberId);
                return AplicationErrors.NotFound("Attendance", memberId);
            }

            return lastAttendance;
        }
        }

        public async Task<Result<List<MemberDto>>> SearchMembers(string name, CancellationToken ct)
        {
            var keyword = name.Trim().ToLower();
            var members = await context.Members
                .AsNoTracking()
                .Where(m => m.FullName!.ToLower().Contains(keyword))
                .Select(m => new MemberDto
                {
                    Id = m.Id,
                    FullName = m.FullName!,
                    Email = m.Email!,
                    Phone = m.Phone!,
                    type = m.memberShips
                        .OrderBy(ms => ms.StartDate)
                        .Select(ms => ms.Type)
                        .LastOrDefault()
                })
                .ToListAsync(ct);
            logger.LogInformation("Searched Member with keyword '{Keyword}'. Found {Count} matches.", keyword, members.Count);

            return members;
        }
        private void ClearMemberCache()
        {
            cache.Remove("members");
            cache.Remove("activemembers");
        }

    }
}

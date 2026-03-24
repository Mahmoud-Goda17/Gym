using Gym.Application.Interfaces;
using Gym.Application.Requests.Member;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class MemberController(IMemberService service) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberRequest request, CancellationToken ct)
        {
            var result = await service.CreateMember(request, ct);
            return result.Match(
                member => CreatedAtAction(nameof(GetMemberById), new { id = member.Id }, member),
                errors => Problem(errors));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMember(UpdateMemberReaquest request, CancellationToken ct)
        {
            var result = await service.UpdateMember(request, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMember(Guid id, CancellationToken ct)
        {
            var result = await service.DeleteMember(id, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMemberById(Guid id, CancellationToken ct)
        {
            var result = await service.GetMemberById(id, ct);
            return result.Match(
                member => Ok(member),
                errors => Problem(errors));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMembers(CancellationToken ct)
        {
            var result = await service.GetAllMembers(ct);
            return result.Match(
                members => Ok(members),
                errors => Problem(errors));
        }
        [HttpGet("actives")]
        public async Task<IActionResult> GetActiveMembers(CancellationToken ct)
        {
            var result = await service.GetActiveMembers(ct);
            return result.Match(
                members => Ok(members),
                errors => Problem(errors));
        }
        [HttpPost("membership")]
        public async Task<IActionResult> AddMemberShip(CreateMemberShipReaquest request, CancellationToken ct)
        {
            var result = await service.AddMemberShip(request, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpPost("{id:guid}/checkin")]
        public async Task<IActionResult> ChickIn(Guid id, CancellationToken ct)
        {
            var result = await service.ChickIn(id, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpPost("{id:guid}/checkout")]
        public async Task<IActionResult> ChickOut(Guid id, CancellationToken ct)
        {
            var result = await service.ChickOut(id, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpPatch("{id:guid}/activate")]
        public async Task<IActionResult> ActivateMember(Guid id, CancellationToken ct)
        {
            var result = await service.ActivateMember(id, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpGet("{id:guid}/attendances")]
        public async Task<IActionResult> GetAttendances(Guid id, CancellationToken ct)
        {
            var result = await service.GetAttendances(id, ct);
            return result.Match(
                attendances => Ok(attendances),
                errors => Problem(errors));
        }
        [HttpGet("{id:guid}/memberships")]
        public async Task<IActionResult> GetMemberships(Guid id, CancellationToken ct)
        {
            var result = await service.GetMemberships(id, ct);
            return result.Match(
                memberships => Ok(memberships),
                errors => Problem(errors));
        }
        [HttpPatch("{memberId:guid}/membership/{membershipId:guid}/freeze")]
        public async Task<IActionResult> FreezMembership(Guid memberId, Guid membershipId, CancellationToken ct)
        {
            var result = await service.FreezMembership(memberId, membershipId, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpPatch("{memberId:guid}/membership/{membershipId:guid}/defreeze")]
        public async Task<IActionResult> DeFreezMembership(Guid memberId, Guid membershipId, CancellationToken ct)
        {
            var result = await service.DeFreezMembership(memberId, membershipId, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpGet("{id:guid}/last-attendance")]
        public async Task<IActionResult> GetLastAttendances(Guid id, CancellationToken ct)
        {
            var result = await service.GetLastAttendances(id, ct);
            return result.Match(
                attendance => Ok(attendance),
                errors => Problem(errors));
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchMembers(string name, CancellationToken ct)
        {
            var result = await service.SearchMembers(name, ct);
            return result.Match(
                members => Ok(members),
                errors => Problem(errors));
        }
    }
    }

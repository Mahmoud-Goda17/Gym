using Gym.Application.Interfaces;
using Gym.Application.Requests.GymClass;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class GymClassController(IGymClassService service) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreateGymClass(CreateGymClassRequest request, CancellationToken ct)
        {
            var result = await service.CreateGymClass(request, ct);
            return result.Match(
                gymClass => CreatedAtAction(nameof(GetGymClassById), new { id = gymClass.Id }, gymClass),
                errors => Problem(errors));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateGymClass(UpdateGymClassRequest request, CancellationToken ct)
        {
            var result = await service.UpdateGymClasses(request, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGymClass(Guid id, CancellationToken ct)
        {
            var result = await service.DeleteGymClass(id, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGymClassById(Guid id, CancellationToken ct)
        {
            var result = await service.GetGymClassById(id, ct);
            return result.Match(
                gymClass => Ok(gymClass),
                errors => Problem(errors));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGymClasses(CancellationToken ct)
        {
            var result = await service.GetAllGymClasses(ct);
            return result.Match(
                gymClasses => Ok(gymClasses),
                errors => Problem(errors));
        }
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpCommingGymClasses(CancellationToken ct)
        {
            var result = await service.GetUpCommingGymClasses(ct);
            return result.Match(
                gymClasses => Ok(gymClasses),
                errors => Problem(errors));
        }
        [HttpGet("trainer/{trainerId:guid}")]
        public async Task<IActionResult> GetGymClassesByTrainerId(Guid trainerId, CancellationToken ct)
        {
            var result = await service.GetGymClassesByTrainerId(trainerId, ct);
            return result.Match(
                gymClasses => Ok(gymClasses),
                errors => Problem(errors));
        }
        [HttpGet("trainer/{trainerId:guid}/income")]
        public async Task<IActionResult> CalculateTrainerIncome(
            Guid trainerId,
            [FromQuery] DateTimeOffset? from,
            [FromQuery] DateTimeOffset? to,
            CancellationToken ct)
        {
            var result = await service.CalculateTrainerIncome(trainerId, from, to, ct);
            return result.Match(
                income => Ok(income),
                errors => Problem(errors));
        }
    }
}


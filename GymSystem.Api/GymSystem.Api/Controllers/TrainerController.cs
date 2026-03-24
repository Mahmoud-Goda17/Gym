using Gym.Application.Interfaces;
using Gym.Application.Requests.Trainer;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TrainerController(ITrainerService service) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> CreateTrainer(CreateTrainerRequest request, CancellationToken ct)
        {
            var result = await service.CreateTrainer(request, ct);
            return result.Match(
                trainer => CreatedAtAction(nameof(GetTrainerById), new { id = trainer.Id }, trainer),
                errors => Problem(errors));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTrainer(UpdateTrainerRequest request, CancellationToken ct)
        {
            var result = await service.UpdateTrainer(request, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTrainer(Guid id, CancellationToken ct)
        {
            var result = await service.DeleteTrainer(id, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTrainerById(Guid id, CancellationToken ct)
        {
            var result = await service.GetTrainerById(id, ct);
            return result.Match(
                trainer => Ok(trainer),
                errors => Problem(errors));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTrainers(CancellationToken ct)
        {
            var result = await service.GetAllTrainers(ct);
            return result.Match(
                trainers => Ok(trainers),
                errors => Problem(errors));
        }
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveTrainers(CancellationToken ct)
        {
            var result = await service.GetActiveTrainers(ct);
            return result.Match(
                trainers => Ok(trainers),
                errors => Problem(errors));
        }
        [HttpPatch("{id:guid}/activate")]
        public async Task<IActionResult> ActivateTrainer(Guid id, CancellationToken ct)
        {
            var result = await service.Activetrainer(id, ct);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchTrainer([FromQuery] string name, CancellationToken ct)
        {
            var result = await service.SearchTrainer(name, ct);
            return result.Match(
                trainers => Ok(trainers),
                errors => Problem(errors));

        }
    }
}


using Gym.Application.Dtos.Trainer;
using Gym.Application.Errors;
using Gym.Application.Interfaces;
using Gym.Application.Mapper.Trainers;
using Gym.Application.Requests.Trainer;
using Gym.Domain.Results;
using Gym.Domain.Trainers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Gym.Application.Services
{
    public class TrainerService(
        ILogger<TrainerService> logger,
        IAppDbContext context,
        IMemoryCache cache) : ITrainerService
    {
        public async Task<Result<TrainerDto>> CreateTrainer(
            CreateTrainerRequest request,
            CancellationToken ct)
        {
            var email = request.Email.Trim().ToLower();
            var phone = request.Phone.Trim();
            var exists = await context.Trainers.AnyAsync(
                           c => c.Email!.ToLower() == email
                           || c.Phone == phone,
                                          ct);
            if (exists)
            {
                logger.LogWarning("Trainer creation aborted. Email or Phone already exists.");
                return TrainerErrors.TrainerExists;
            }
            var trainerResult = Trainer.Create(
               Guid.NewGuid(),
               request.FullName.Trim(),
               request.Email.Trim(),
               request.Phone.Trim(),
               request.HourlyRate
                );
            if (trainerResult.IsError)
            {
                logger.LogWarning(
                    "Trainer creation failed due to validation errors: {Errors}",
                    trainerResult.Errors);
                return trainerResult.Errors;
            }
            var newtrainer= trainerResult.Value;
            context.Trainers.Add(newtrainer);
            await context.SaveChangesAsync(ct);
            ClearTrainerCache();
            logger.LogInformation(
                "Trainer created successfully with ID: {TrainerId}",
                newtrainer.Id);
            return newtrainer.ToDto();

        }
        public async Task<Result<Updated>> UpdateTrainer(
            UpdateTrainerRequest request,
            CancellationToken ct)
        {
            var email = request.Email.Trim().ToLower();
            var phone = request.Phone.Trim();
            var exists = await context.Trainers.AnyAsync(c => c.Id != request.Id &&
                          (
                          c.Email!.ToLower() == email || c.Phone == phone
                          ),
                          ct);
            if (exists)
            {
                logger.LogWarning("Trainer update aborted. Email or Phone already exists for another trainer.");
                return TrainerErrors.TrainerExists;
            }
          var trainer=  await context.Trainers
                       .FirstOrDefaultAsync(t => t.Id == request.Id, ct);
            if (trainer == null)
            {
                logger.LogWarning(
                    "Trainer update failed. Trainer with ID {TrainerId} not found.",
                     request.Id);
                return AplicationErrors.NotFound("Trainer", request.Id);
            }
            var updateResult = trainer.Update(
                request.FullName.Trim(),
                request.Email.Trim(),
                request.Phone.Trim(),
                request.HourlyRate);
            if (updateResult.IsError)
            {
                logger.LogWarning(
                    "Trainer update failed due to validation errors: {Errors}",
                    updateResult.Errors);
                return updateResult.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearTrainerCache();
            logger.LogInformation(
                "Trainer with ID {TrainerId} updated successfully.",
                trainer.Id);
            return Result.Updated;
        }

        public async Task<Result<Deleted>> DeleteTrainer(
            Guid id,
            CancellationToken ct)
        {
            var trainer = await context.Trainers
                         .FirstOrDefaultAsync(t => t.Id == id, ct);
            if (trainer == null)
            {
                logger.LogWarning(
                    " Trainer with ID {TrainerId} not found.",
                    id);
                return AplicationErrors.NotFound("Trainer", id);
            }
            var deleteResult = trainer.Deactivate();
            if (deleteResult.IsError)
            {
                logger.LogWarning("Trainer deletion failed. Trainer with ID {TrainerId} is already inactive.", id);
                return deleteResult.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearTrainerCache();
            logger.LogInformation
                ("Trainer with ID {TrainerId} marked as inactive successfully.",
                id);

            return Result.Deleted;
        }

        public async Task<Result<List<TrainerDto>>> GetActiveTrainers(CancellationToken ct)
        {
            var cacheKey = "ActiveTrainers";
            var trainers = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.Size = 1;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                var trainerresponse = await context.Trainers
                                      .Where(t => t.IsActive == true)
                                      .AsNoTracking()
                                      .ToListAsync();
                return trainerresponse.ToDtos();

            });
            logger.LogInformation(
                "Retrieved active trainers from cache or database. Count: {Count}",
                trainers.Count);
            return trainers;
        }

        public async Task<Result<List<TrainerDto>>> GetAllTrainers(CancellationToken ct)
        {
            var cacheKey = "Trainers";
            var trainers = await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.Size = 1;
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                var trainerresponse = await context.Trainers
                                      .AsNoTracking()
                                      .ToListAsync();
                return trainerresponse.ToDtos();

            });
            logger.LogInformation(
                "Retrieved active trainers from cache or database. Count: {Count}",
                 trainers.Count);
            return trainers;

        }

        public async Task<Result<TrainerDto>> GetTrainerById(
            Guid id,
            CancellationToken ct)
        {
            var trainer = await context.Trainers
                         .FirstOrDefaultAsync(t => t.Id == id, ct);
            if (trainer == null)
            {
                logger.LogWarning(
                    " Trainer with ID {TrainerId} not found.",
                    id);
                return AplicationErrors.NotFound("Trainer", id);
            }
            logger.LogInformation(
                "Retrieved trainer with ID {TrainerId} successfully.",
                id);
            return trainer.ToDto();
        }

        public async Task<Result<Updated>> Activetrainer(
            Guid id,
             CancellationToken ct)
        {
            var trainer = await context.Trainers
                         .FirstOrDefaultAsync(t => t.Id == id, ct);
            if (trainer == null)
            {
                logger.LogWarning(
                    " Trainer with ID {TrainerId} not found.",
                    id);
                return AplicationErrors.NotFound("Trainer", id);
            }
            var activateResult = trainer.Activate();
            if (activateResult.IsError)
            {
                logger.LogWarning("Trainer activation failed. Trainer with ID {TrainerId} is already active.", id);
                return activateResult.Errors;
            }
            await context.SaveChangesAsync(ct);
            ClearTrainerCache();
            logger.LogInformation(
                "Trainer with ID {TrainerId} activated successfully.",
                id);
            return Result.Updated;
        }

        public async Task<Result<List<TrainerDto>>> SearchTrainer(
            string name,
             CancellationToken ct)
        {
            var keyword = name.Trim().ToLower();
            var Trainers = await context.Trainers
                .AsNoTracking()
                .Where(m => m.FullName!.ToLower().Contains(keyword))
                .ToListAsync(ct);
            logger.LogInformation(
                "Searched trainers with keyword '{Keyword}'. Found {Count} matches.",
                keyword, Trainers.Count);
            return Trainers.ToDtos();

        }
        private void ClearTrainerCache()
        {
            cache.Remove("Trainers");
            cache.Remove("ActiveTrainers");
        }

    }
}

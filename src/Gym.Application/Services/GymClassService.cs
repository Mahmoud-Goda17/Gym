
using Gym.Application.Dtos.GymClass;
using Gym.Application.Errors;
using Gym.Application.Interfaces;
using Gym.Application.Mapper.GymClasss;
using Gym.Application.Requests.GymClass;
using Gym.Domain.GymClasss;
using Gym.Domain.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Gym.Application.Services
{
    namespace Gym.Application.Services
    {
        public class GymClassService(
            IAppDbContext dbContext,
            ILogger<GymClassService> logger,
            IMemoryCache cache) : IGymClassService
        {
            public async Task<Result<decimal>> CalculateTrainerIncome(
                Guid trainerId,
                DateTimeOffset? from,
                DateTimeOffset? to,
                CancellationToken ct)
            {
                var trainer = await dbContext.Trainers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == trainerId, ct);

                if (trainer == null)
                {
                    logger.LogWarning("Trainer {TrainerId} not found for income calculation.", trainerId);
                    return AplicationErrors.NotFound("Trainer", trainerId);
                }

                logger.LogInformation("Calculating income for trainer {TrainerId}.", trainerId);

                var now = DateTimeOffset.UtcNow;

                var query = dbContext.GymClasses
                    .AsNoTracking()
                    .Where(c =>
                        c.TrainerId == trainerId &&
                        c.EndTime <= now);

                if (from.HasValue)
                    query = query.Where(c => c.StartTime >= from.Value);

                if (to.HasValue)
                    query = query.Where(c => c.EndTime <= to.Value);

                var classes = await query.ToListAsync(ct);

                decimal totalIncome = 0;

                foreach (var gymClass in classes)
                {
                    var duration = gymClass.EndTime - gymClass.StartTime;
                    var hours = (decimal)duration.TotalHours;
                    totalIncome += hours * trainer.HourlyRate;
                }

                logger.LogInformation(
                    "Income calculated for trainer {TrainerId}. Total Income: {TotalIncome}",
                    trainerId,
                    totalIncome);

                return totalIncome;
            }

            public async Task<Result<GymClassDto>> CreateGymClass(
                CreateGymClassRequest request,
                CancellationToken ct)
            {
                var trainerExists = await dbContext.Trainers
                    .AnyAsync(t => t.Id == request.TrainerId && t.IsActive, ct);

                if (!trainerExists)
                {
                    logger.LogWarning("Trainer {TrainerId} not found or inactive.", request.TrainerId);
                    return AplicationErrors.NotFound("Trainer", request.TrainerId);
                }

                var isBusy = await IsTrainerBusy(
                    request.TrainerId,
                    request.StartTime,
                    request.EndTime,
                    null,
                    ct);

                if (isBusy)
                {
                    logger.LogWarning(
                        "Scheduling conflict for trainer {TrainerId} between {Start} and {End}.",
                        request.TrainerId,
                        request.StartTime,
                        request.EndTime);

                    return GymClassErrors.NotAvalibleTrainer;
                }

                var result = GymClass.Created(
                    Guid.NewGuid(),
                    request.Name,
                    request.TrainerId,
                    request.StartTime,
                    request.EndTime,
                    request.Capacity);

                if (result.IsError)
                {
                    logger.LogWarning("GymClass creation failed: {Errors}", result.Errors);
                    return result.Errors;
                }

                dbContext.GymClasses.Add(result.Value);
                await dbContext.SaveChangesAsync(ct);

                ClearGymClassCache();

                logger.LogInformation(
                    "GymClass {GymClassId} created successfully for trainer {TrainerId}.",
                    result.Value.Id,
                    request.TrainerId);

                return result.Value.ToDto();
            }

            public async Task<Result<Updated>> UpdateGymClasses(
                UpdateGymClassRequest request,
                CancellationToken ct)
            {
                var existing = await dbContext.GymClasses
                    .FirstOrDefaultAsync(gc => gc.Id == request.Id, ct);

                if (existing == null)
                {
                    logger.LogWarning("GymClass {GymClassId} not found for update.", request.Id);
                    return AplicationErrors.NotFound("GymClass", request.Id);
                }

                var trainerExists = await dbContext.Trainers
                    .AnyAsync(t => t.Id == request.TrainerId && t.IsActive, ct);

                if (!trainerExists)
                {
                    logger.LogWarning("Trainer {TrainerId} not found or inactive.", request.TrainerId);
                    return AplicationErrors.NotFound("Trainer", request.TrainerId);
                }

                var isBusy = await IsTrainerBusy(
                    request.TrainerId,
                    request.StartTime,
                    request.EndTime,
                    request.Id,
                    ct);

                if (isBusy)
                {
                    logger.LogWarning(
                        "Update conflict for trainer {TrainerId} between {Start} and {End}.",
                        request.TrainerId,
                        request.StartTime,
                        request.EndTime);

                    return GymClassErrors.NotAvalibleTrainer;
                }

                var updateResult = existing.Update(
                    request.Name,
                    request.TrainerId,
                    request.StartTime,
                    request.EndTime,
                    request.Capacity);

                if (updateResult.IsError)
                {
                    logger.LogWarning(
                        "GymClass {GymClassId} update failed: {Errors}",
                        request.Id,
                        updateResult.Errors);

                    return updateResult.Errors;
                }

                await dbContext.SaveChangesAsync(ct);

                ClearGymClassCache();

                logger.LogInformation("GymClass {GymClassId} updated successfully.", request.Id);

                return Result.Updated;
            }

            public async Task<Result<Deleted>> DeleteGymClass(Guid id, CancellationToken ct)
            {
                var existing = await dbContext.GymClasses
                    .FirstOrDefaultAsync(gc => gc.Id == id, ct);

                if (existing == null)
                {
                    logger.LogWarning("GymClass {GymClassId} not found for deletion.", id);
                    return AplicationErrors.NotFound("GymClass", id);
                }

                dbContext.GymClasses.Remove(existing);
                await dbContext.SaveChangesAsync(ct);

                ClearGymClassCache();

                logger.LogInformation("GymClass {GymClassId} deleted successfully.", id);

                return Result.Deleted;
            }

            public async Task<Result<List<GymClassDto>>> GetAllGymClasses(CancellationToken ct)
            {
                var cacheKey = "GymClasses";

                var classes = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.Size = 1;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                    var list = await dbContext.GymClasses
                        .AsNoTracking()
                        .ToListAsync(ct);

                    return list.ToDtos();
                });

                return classes;
            }

            public async Task<Result<List<GymClassDto>>> GetUpCommingGymClasses(CancellationToken ct)
            {
                var cacheKey = "UpCommingGymClasses";

                var classes = await cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.Size = 1;
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);

                    var list = await dbContext.GymClasses
                        .AsNoTracking()
                        .Where(gc => gc.StartTime > DateTimeOffset.UtcNow)
                        .ToListAsync(ct);

                    return list.ToDtos();
                });

                return classes;
            }

            public async Task<Result<List<GymClassDto>>> GetGymClassesByTrainerId(
                Guid trainerId,
                CancellationToken ct)
            {
                var trainerExists = await dbContext.Trainers
                    .AnyAsync(t => t.Id == trainerId && t.IsActive, ct);

                if (!trainerExists)
                {
                    logger.LogWarning("Trainer {TrainerId} not found or inactive.", trainerId);
                    return AplicationErrors.NotFound("Trainer", trainerId);
                }

                var classes = await dbContext.GymClasses
                    .AsNoTracking()
                    .Where(gc => gc.TrainerId == trainerId)
                    .OrderBy(gc => gc.StartTime)
                    .ToListAsync(ct);

                return classes.ToDtos();
            }
            public async Task<Result<GymClassDto>> GetGymClassById(Guid id, CancellationToken ct)
            {
                var existingGymClass = await dbContext.GymClasses
                    .FirstOrDefaultAsync(gc => gc.Id == id, ct);
                if (existingGymClass == null)
                {
                    logger.LogWarning("Gym class with ID {GymClassId} not found ", id);
                    return AplicationErrors.NotFound("GymClass", id);
                }
                logger.LogInformation("Gym class with ID {GymClassId} retrieved successfully.", id);
                return existingGymClass.ToDto();
            }

            private Task<bool> IsTrainerBusy(
                Guid trainerId,
                DateTimeOffset start,
                DateTimeOffset end,
                Guid? ignoreClassId,
                CancellationToken ct)
            {
                return dbContext.GymClasses.AnyAsync(gc =>
                    gc.TrainerId == trainerId &&
                    (ignoreClassId == null || gc.Id != ignoreClassId) &&
                    start < gc.EndTime &&
                    end > gc.StartTime,
                    ct);
            }

            private void ClearGymClassCache()
            {
                cache.Remove("GymClasses");
                cache.Remove("UpCommingGymClasses");
            }
        }
    }
}

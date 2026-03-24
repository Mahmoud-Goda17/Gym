
using Gym.Application.Interfaces;
using Gym.Domain.Members.Memberships.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gym.Infrastrucure.BackgroundJobs
{
    public class MembershipExpiredJob(
        IServiceScopeFactory scopeFactory,
        ILogger<MembershipExpiredJob> logger,
        TimeProvider dateTime) : BackgroundService

    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly ILogger<MembershipExpiredJob> _logger = logger;
        private readonly TimeProvider _dateTime = dateTime;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Checking Membership Expired at {Now}", _dateTime.GetUtcNow());
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                    var expiredMemberships = await db.MemberShips
                        .Where(m => m.Status == MembershipStatus.Active
                               && m.EndDate <= _dateTime.GetUtcNow())
                              .ToListAsync(stoppingToken);
                    if (expiredMemberships.Any())
                    {
                        foreach (var membership in expiredMemberships)
                        {
                            var result = membership.Expire();
                            if (result.IsError)
                            {
                                _logger.LogWarning("Failed to cancel expired membership {Id}: {Error}", membership.Id, result.Errors);
                            }

                        }
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Updated {Count} expired memberships.", expiredMemberships.Count);
                    }
                    else
                    {
                        _logger.LogInformation("No expired memberships found.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking for expired memberships.");

                }
            }
        }
    }
}


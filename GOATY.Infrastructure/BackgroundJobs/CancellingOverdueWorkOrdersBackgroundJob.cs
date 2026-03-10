using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GOATY.Infrastructure.BackgroundJobs
{
    public sealed class CancellingOverdueWorkOrdersBackgroundJob(
        IServiceScopeFactory scopeFactory,
        ILogger<CancellingOverdueWorkOrdersBackgroundJob> logger)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(10));

            var cutoff = DateTimeOffset.UtcNow.AddMinutes(-10);

            try
            {
                while(await periodicTimer.WaitForNextTickAsync(stoppingToken))
                {
                    using var scope = scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                    var workOrders = await context.WorkOrders
                        .Where(wo => wo.State == State.Scheduled &&wo.StartTime <= cutoff)
                        .ToListAsync();
                    
                    if(workOrders.Count > 0)
                    {
                        foreach (var workOrder in workOrders)
                        {
                            var updatedResult = workOrder.UpdateState(State.Cancelled);

                            if (!updatedResult.IsSuccess)
                            {
                                logger.LogWarning("Failed to cancel WorkOrder {Id}: {Error}"
                                    ,workOrder.Id, updatedResult.Errors);
                            }
                        }

                        await context.SaveChangesAsync(stoppingToken);

                        logger.LogWarning("Cancelled {Count} scheduled work orders that exceeded the allowed waiting time."
                            , workOrders.Count);
                    }
                    else
                    {
                        logger.LogInformation("No scheduled work orders required cancellation.");
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Cancelling work orders background service is stopping.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred in cancelling work orders background service.");
            }
        }
    }
}

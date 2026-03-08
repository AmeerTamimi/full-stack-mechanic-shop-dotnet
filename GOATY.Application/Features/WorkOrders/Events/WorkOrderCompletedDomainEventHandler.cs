using GOATY.Application.Common.Interfaces;
using GOATY.Domain.WorkOrders.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.Events
{
    public sealed class WorkOrderCompletedDomainEventHandler(
        IAppDbContext context,
        ILogger<WorkOrderCompletedDomainEventHandler> logger,
        INotificationService notificationService)
        : INotificationHandler<WorkOrderCompletedDomainEvent>
    {
        public async Task Handle(WorkOrderCompletedDomainEvent notification, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                .Include(wo => wo.Customer)
                .FirstOrDefaultAsync(wo => wo.Id == notification.WorkOrderId, ct);

            if (workOrder is null)
            {
                logger.LogWarning(
                    "Work order {WorkOrderId} was not found while handling completion event.",
                    notification.WorkOrderId);

                return;
            }

            if (workOrder.Customer is null)
            {
                logger.LogWarning(
                    "Customer was not found for completed work order {WorkOrderId}.",
                    notification.WorkOrderId);

                return;
            }

            await notificationService.SendEmailAsync(workOrder.Customer, ct);
            await notificationService.SendSmsAsync(workOrder.Customer, ct);

            logger.LogInformation(
                "Completion notifications were sent for work order {WorkOrderId}.",
                notification.WorkOrderId);
        }
    }
}
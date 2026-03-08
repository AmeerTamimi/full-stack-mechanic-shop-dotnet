using GOATY.Domain.WorkOrders.Events;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.Events
{
    public sealed class WorkOrderCreatedDomainEventHandler
         : INotificationHandler<WorkOrderCreatedDomainEvent>
    {
        public Task Handle(WorkOrderCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // idk brother
            return Task.CompletedTask;
        }
    }
}

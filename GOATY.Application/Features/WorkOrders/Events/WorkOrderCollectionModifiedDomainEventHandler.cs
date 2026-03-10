using GOATY.Application.Common.Interfaces;
using GOATY.Domain.WorkOrders.Events;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.Events
{
    public sealed class WorkOrderCollectionModifiedDomainEventHandler(
        IWorkOrderNotifier workOrderNotifier)
        : INotificationHandler<WorkOrderCollectionModifiedDomainEvent>
    {
        public async Task Handle(WorkOrderCollectionModifiedDomainEvent notification, CancellationToken ct)
             => await workOrderNotifier.NotifyWorkOrdersChangedAsync(ct);
        
    }
}

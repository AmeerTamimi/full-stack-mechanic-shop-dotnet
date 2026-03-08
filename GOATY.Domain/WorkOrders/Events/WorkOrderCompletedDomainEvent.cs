using GOATY.Domain.Common;

namespace GOATY.Domain.WorkOrders.Events
{
    public sealed class WorkOrderCompletedDomainEvent: DomainEvent
    {
        public Guid WorkOrderId { get; set; }
    }
}

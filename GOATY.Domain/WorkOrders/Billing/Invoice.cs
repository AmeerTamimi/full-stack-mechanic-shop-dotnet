using GOATY.Domain.Common;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Domain.WorkOrders.Billing
{
    public sealed class Invoice : AuditableEntity
    {
        public decimal DiscoutAmout { get; private set; }
        public DateTimeOffset IssuedAt { get; private set; }
        public DateTimeOffset PaidAt { get; private set; }
        public Status Status { get; private set; }
        public decimal TotalTax { get; private set; }
        public decimal TotalPrice { get; private set; }
        public Guid WorkOrderId { get; private set; }
        public WorkOrder WorkOrder { get; private set; }

        private Invoice() { }

        private Invoice(
            Guid id,
            decimal discoutAmout,
            DateTimeOffset issuedAt,
            DateTimeOffset paidAt,
            Status status,
            decimal totalTax,
            decimal totalPrice,
            Guid workOrderId,
            WorkOrder workOrder) : base(id)
        {
            Id = id;
            DiscoutAmout = discoutAmout;
            IssuedAt = issuedAt;
            PaidAt = paidAt;
            Status = status;
            TotalTax = totalTax;
            TotalPrice = totalPrice;
            WorkOrderId = workOrderId;
            WorkOrder = workOrder;
        }
    }
}

using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;

namespace GOATY.Domain.WorkOrders.Billing
{
    public sealed class Invoice : AuditableEntity
    {
        public DateTimeOffset IssuedAt { get; private set; }
        public DateTimeOffset PaidAt { get; private set; }
        public Status Status { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal Discount { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }
        public Guid WorkOrderId { get; private set; }
        public WorkOrder WorkOrder { get; private set; }

        private readonly List<InvoiceItem> _invoiceItems;
        public IReadOnlyCollection<InvoiceItem> InvoiceItems => _invoiceItems;

        private Invoice() { }

        private Invoice(
            Guid id,
            decimal discout,
            DateTimeOffset issuedAt,
            DateTimeOffset paidAt,
            Status status,
            decimal tax,
            decimal total,
            Guid workOrderId) : base(id)
        {
            Id = id;
            Discount = discout;
            IssuedAt = issuedAt;
            PaidAt = paidAt;
            Status = status;
            Tax = tax;
            Total = total;
            WorkOrderId = workOrderId;
        }

        public static Result<Invoice> Create(Guid id,
                                             decimal discout,
                                             DateTimeOffset issuedAt,
                                             Guid workOrderId)
        {

        }
    }
}

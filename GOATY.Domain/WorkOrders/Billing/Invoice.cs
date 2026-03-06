using GOATY.Domain.Common;
using GOATY.Domain.Common.Constans;
using GOATY.Domain.Common.Results;

namespace GOATY.Domain.WorkOrders.Billing
{
    public sealed class Invoice : AuditableEntity
    {
        public DateTimeOffset IssuedAt { get; private set; }
        public DateTimeOffset? PaidAt { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal Discount { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }
        public Guid WorkOrderId { get; private set; }
        public WorkOrder WorkOrder { get; private set; }

        private readonly List<InvoiceItem> _invoiceItems;
        public IReadOnlyCollection<InvoiceItem> InvoiceItems => _invoiceItems;
        public bool IsEditable => Status != InvoiceStatus.Refunded;

        private Invoice() { }

        private Invoice(
            Guid id,
            decimal discount,
            DateTimeOffset issuedAt,
            decimal tax,
            decimal total,
            Guid workOrderId,
            List<InvoiceItem> invoiceItems) : base(id)
        {
            Id = id;
            Discount = discount;
            IssuedAt = issuedAt;
            Status = InvoiceStatus.NotPayed;
            Tax = tax;
            Total = total;
            WorkOrderId = workOrderId;
            _invoiceItems = invoiceItems;
        }

        public static Result<Invoice> Create(Guid id,
                                             decimal discount,
                                             DateTimeOffset issuedAt,
                                             decimal total,
                                             Guid workOrderId,
                                             List<InvoiceItem> invoiceItems)
        {
            if(Guid.Empty == id)
            {
                return InvoiceErrors.InvalidId;
            }
            if (discount < 0)
            {
                return InvoiceErrors.InvalidDiscount;
            }
            if (issuedAt.Date > DateTimeOffset.Now)
            {
                return InvoiceErrors.InvalidIssuedDate;
            }
            if (total < 0)
            {
                return InvoiceErrors.InvalidTotalPrice;
            }
            if (Guid.Empty == workOrderId)
            {
                return InvoiceErrors.InvalidWorkOrderId;
            }
            if (invoiceItems is null || invoiceItems.Count == 0)
            {
                return InvoiceErrors.InvalidInvoiceItems;
            }

            var tax = GOATYConstans.TaxRate * total;

            return new Invoice(id , discount , issuedAt , tax, total , workOrderId , invoiceItems);
        }

        public Result<Updated> UpdatePayStatus(InvoiceStatus newStatus)
        {
            if (!IsEditable)
            {
                return InvoiceErrors.InvoiceNotEditable;
            }

            if (!Enum.IsDefined(typeof(InvoiceStatus), newStatus))
            {
                return InvoiceErrors.InvalidStatus;
            }

            if(!ValidStatusTransition(Status , newStatus))
            {
                return InvoiceErrors.InvalidStatusTransition;
            }

            Status = newStatus;

            if(Status == InvoiceStatus.Payed) // refuneded case
            {
                PaidAt = DateTimeOffset.Now;
            }

            return Result.Updated;
        }

        private bool ValidStatusTransition(InvoiceStatus currentStatus , InvoiceStatus newStatus)
        {
            return currentStatus == InvoiceStatus.NotPayed && newStatus == InvoiceStatus.Payed ||
                   currentStatus == InvoiceStatus.Payed && newStatus == InvoiceStatus.Refunded;
        }
    }
}

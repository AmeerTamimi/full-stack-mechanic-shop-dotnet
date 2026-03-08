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
            DateTimeOffset issuedAt,
            decimal subTotal,
            decimal tax,
            decimal total,
            Guid workOrderId,
            List<InvoiceItem> invoiceItems) : base(id)
        {
            Id = id;
            IssuedAt = issuedAt;
            SubTotal = subTotal;
            Status = InvoiceStatus.NotPayed;
            Tax = tax;
            Total = total;
            WorkOrderId = workOrderId;
            _invoiceItems = invoiceItems;
        }

        public static Result<Invoice> Create(Guid id,
                                             Guid workOrderId,
                                             decimal discount,
                                             List<InvoiceItem> invoiceItems)
        {
            if(Guid.Empty == id)
            {
                return InvoiceErrors.InvalidId;
            }
            if (discount < 0 || discount > 100)
            {
                return InvoiceErrors.InvalidDiscount;
            }
            if (Guid.Empty == workOrderId)
            {
                return InvoiceErrors.InvalidWorkOrderId;
            }
            if (invoiceItems is null || invoiceItems.Count == 0)
            {
                return InvoiceErrors.InvalidInvoiceItems;
            }

            var subtotal = invoiceItems.Sum(it => it.Total);
            var tax = GOATYConstans.TaxRate * subtotal;
            var total = subtotal + tax - (discount/100 * (subtotal + tax));

            return new Invoice(id , DateTimeOffset.Now , subtotal,tax, total , workOrderId , invoiceItems);
        }

        public Result<Updated> PayInvoice()
        {
            if (!IsEditable)
            {
                return InvoiceErrors.InvoiceNotEditable;
            }

            if(!ValidStatusTransition(Status , InvoiceStatus.Payed))
            {
                return InvoiceErrors.InvalidStatusTransition;
            }

            Status = InvoiceStatus.Payed;
            PaidAt = DateTimeOffset.Now;

            return Result.Updated;
        }

        public Result<Updated> RefundInvoice()
        {
            if (!IsEditable)
            {
                return InvoiceErrors.InvoiceNotEditable;
            }

            if (!ValidStatusTransition(Status, InvoiceStatus.Refunded))
            {
                return InvoiceErrors.InvalidStatusTransition;
            }

            Status = InvoiceStatus.Refunded;

            return Result.Updated;
        }
        private bool ValidStatusTransition(InvoiceStatus currentStatus , InvoiceStatus newStatus)
        {
            return currentStatus == InvoiceStatus.NotPayed && newStatus == InvoiceStatus.Payed ||
                   currentStatus == InvoiceStatus.Payed && newStatus == InvoiceStatus.Refunded;
        }
    }
}

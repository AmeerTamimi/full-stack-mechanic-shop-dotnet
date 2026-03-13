using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Billing;

namespace GOATY.Tests.Common.WorkOrders.Invoices
{
    public static class InvoiceFactory
    {
        public static Result<Invoice> Create(Guid? id = null,
                                             Guid? workOrderId = null,
                                             decimal? discount = null,
                                             List<InvoiceItem>? invoiceItems = null)
        {
            return Invoice.Create(
                id ?? Guid.NewGuid(),
                workOrderId ?? Guid.NewGuid(),
                discount ?? 10,
                invoiceItems ?? [InvoiceItemFactory.Create().Value]);
        }
    }
}

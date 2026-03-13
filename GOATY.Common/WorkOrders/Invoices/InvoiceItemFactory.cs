using GOATY.Domain.Common.Constans;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Billing;

namespace GOATY.Tests.Common.WorkOrders.Invoices
{
    public static class InvoiceItemFactory
    {
        public static Result<InvoiceItem> Create(Guid? id = null,
                                                 Guid? invoiceId = null,
                                                 decimal? technicianCost = null,
                                                 int? quantity = null,
                                                 decimal? unitPrice = null,
                                                 string? description = null,
                                                 Guid? repairTaskId = null,
                                                 Guid? partId = null)
        {
            return InvoiceItem.Create(
                id ?? Guid.NewGuid(),
                invoiceId ?? Guid.NewGuid(),
                technicianCost ?? GOATYConstans.TechnicianBase,
                quantity ?? 1,
                unitPrice ?? 50m,
                description ?? "Hi",
                repairTaskId ?? Guid.NewGuid(),
                partId);
        }
    }
}

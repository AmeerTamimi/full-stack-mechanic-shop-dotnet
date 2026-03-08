using GOATY.Domain.WorkOrders.Billing;

namespace GOATY.Application.Common.Interfaces
{
    public interface IInvoicePdfGenerator
    {
        byte[] Generate(Invoice invoice);
    }
}

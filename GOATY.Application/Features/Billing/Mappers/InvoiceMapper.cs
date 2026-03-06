using GOATY.Application.Features.Billing.DTOs;
using GOATY.Domain.WorkOrders.Billing;

namespace GOATY.Application.Features.Billing.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceDto ToDto(this Invoice model)
        {
            return new InvoiceDto
            {
                IssuedAt = model.IssuedAt,
                PaidAt = model.PaidAt,
                Status = model.Status,
                SubTotal = model.SubTotal,
                Discount = model.Discount,
                Tax = model.Tax,
                Total = model.Total,
                WorkOrderId = model.WorkOrderId,
                Items = model.InvoiceItems.ToList().ToDtos()
            };
        }
        public static List<InvoiceDto> ToDtos(this List<Invoice> models)
        {
            return models.ConvertAll(m => m.ToDto());
        }
    }
}

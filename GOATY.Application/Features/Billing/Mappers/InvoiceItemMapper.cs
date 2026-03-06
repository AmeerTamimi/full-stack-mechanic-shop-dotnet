using GOATY.Application.Features.Billing.DTOs;
using GOATY.Domain.WorkOrders.Billing;

namespace GOATY.Application.Features.Billing.Mappers
{
    public static class InvoiceItemMapper
    {
        public static InvoiceItemDto ToDto(this InvoiceItem model)
        {
            return new InvoiceItemDto
            {
                Id = model.Id,
                TechnicianCost = model.TechnicianCost,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                Total = model.Total,
                InvoiceId = model.InvoiceId,
                RepairTaskId = model?.RepairTaskId,
                PartId = model?.PartId
            };
        }
        public static List<InvoiceItemDto> ToDtos(this List<InvoiceItem> models)
        {
            return models.ConvertAll(m => m.ToDto());
        }

    }
}

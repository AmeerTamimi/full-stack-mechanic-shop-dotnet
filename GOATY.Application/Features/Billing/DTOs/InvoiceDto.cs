using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Billing;

namespace GOATY.Application.Features.Billing.DTOs
{
    public sealed class InvoiceDto
    {
        public DateTimeOffset IssuedAt { get; set; }
        public DateTimeOffset? PaidAt { get; set; }
        public InvoiceStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public Guid WorkOrderId { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = [];
    }
}

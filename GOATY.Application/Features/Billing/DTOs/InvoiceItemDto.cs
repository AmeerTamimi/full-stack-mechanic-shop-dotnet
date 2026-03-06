using GOATY.Domain.Parts;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders.Billing;
namespace GOATY.Application.Features.Billing.DTOs
{
    public sealed class InvoiceItemDto
    {
        public Guid Id { get; set; }
        public int TechnicianCost { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public decimal Total { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid? RepairTaskId { get; set; }
        public Guid? PartId { get; set; }
    }
}

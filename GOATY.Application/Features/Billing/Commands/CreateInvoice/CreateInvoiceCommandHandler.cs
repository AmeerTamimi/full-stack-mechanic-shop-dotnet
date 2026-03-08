using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Billing.DTOs;
using GOATY.Application.Features.Billing.Mappers;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Billing;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Billing.Commands.CreateInvoice
{
    public sealed class CreateInvoiceCommandHandler(
        IAppDbContext context,
        ILogger<CreateInvoiceCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>
    {
        public async Task<Result<InvoiceDto>> Handle(CreateInvoiceCommand request, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                .AsNoTracking()
                .Include(wo => wo.WorkOrderRepairTasks)
                    .ThenInclude(wr => wr.RepairTask)
                        .ThenInclude(r => r.RepairTaskDetails)
                .Include(wo => wo.Invoice)
                .SingleOrDefaultAsync(w => w.Id == request.WorkOrderId, ct);

            if (workOrder is null)
            {
                return Error.NotFound(
                    code: "WorkOrder_NotFound",
                    description: $"WorkOrder With Id {request.WorkOrderId} was Not Found"
                );
            }

            if (workOrder.State != State.Completed)
            {
                return ApplicationErrors.WorkOrderNotCompleted;
            }

            if (workOrder.Invoice is not null)
            {
                return ApplicationErrors.WorkOrderAlreadyHasInvoice;
            }

            var invoiceId = Guid.NewGuid();

            var invoiceItems = new List<InvoiceItem>();

            foreach(var workOrderRepairTask in workOrder.WorkOrderRepairTasks)
            {
                var repairTask = workOrderRepairTask.RepairTask;

                var invoiceItem = InvoiceItem.Create(Guid.NewGuid(),
                                                     invoiceId,
                                                     repairTask.TechnicianCost,
                                                     workOrderRepairTask.Quantity,
                                                     workOrderRepairTask.Cost,
                                                     repairTask.Id,
                                                     null);

                if (!invoiceItem.IsSuccess)
                {
                    return invoiceItem.Errors;
                };

                invoiceItems.Add(invoiceItem.Value);
            }

            var invoice = Invoice.Create(invoiceId,
                                         request.WorkOrderId,
                                         workOrder.Discount,
                                         invoiceItems);

            if (!invoice.IsSuccess)
            {
                return invoice.Errors;
            }

            await context.Invoices.AddAsync(invoice.Value);
            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("invoices", ct);

            return invoice.Value.ToDto();
        }
    }
}

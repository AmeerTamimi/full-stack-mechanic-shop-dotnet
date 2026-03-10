using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.Mappers;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using GOATY.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.DeleteWorkOrder
{
    public sealed class DeleteWorkOrderCommandHandler(
        IAppDbContext context,
        ILogger<DeleteWorkOrderCommandHandler> logger,
        HybridCache cache)
        : IRequestHandler<DeleteWorkOrderCommand, Result<WorkOrderDto>>
    {
        public async Task<Result<WorkOrderDto>> Handle(DeleteWorkOrderCommand request, CancellationToken ct)
        {

            var workOrder = await context.WorkOrders
                .Include(wo => wo.WorkOrderRepairTasks)
                .SingleOrDefaultAsync(wo => wo.Id == request.Id, ct);

            if (workOrder is null)
            {
                return Error.NotFound(
                    code: "WorkOrder_NotFound",
                    description: $"WorkOrder With Id {request.Id} was Not Found"
                );
            }

            if (workOrder.State == State.InProgress)
            {
                return ApplicationErrors.CannotRemoveWorkOrderInProgress;
            }

            context.WorkOrders.Remove(workOrder);
            await context.SaveChangesAsync(ct);

            workOrder.AddDomainEvent(new WorkOrderCollectionModifiedDomainEvent());

            await cache.RemoveByTagAsync("work-orders");

            return workOrder.ToDto();
        }
    }
}
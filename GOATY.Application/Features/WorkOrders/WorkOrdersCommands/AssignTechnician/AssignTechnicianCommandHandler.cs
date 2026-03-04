using GOATY.Application.Common;
using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.AssignTechnician
{
    public sealed class AssignTechnicianCommandHandler(
        IAppDbContext context,
        ILogger<AssignTechnicianCommandHandler> logger,
        HybridCache cache,
        IWorkOrderRules _workOrderRules)
        : IRequestHandler<AssignTechnicianCommand, Result<Updated>>
    {

        public async Task<Result<Updated>> Handle(AssignTechnicianCommand request, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                .SingleOrDefaultAsync(wo => wo.Id == request.WorkOrderId, ct);

            if (workOrder is null)
            {
                return Error.NotFound(
                    code: "WorkOrder_NotFound",
                    description: $"WorkOrder With Id {request.WorkOrderId} was Not Found"
                );
            }

            var employee = await context.Employees
                .SingleOrDefaultAsync(e => e.Id == request.EmployeeId, ct);

            if (employee is null)
            {
                return Error.NotFound(
                    code: "Employee_NotFound",
                    description: $"Employee With Id {request.EmployeeId} was Not Found"
                );
            }

            if (_workOrderRules.IsWorkOrderOccupied(request.WorkOrderId))
            {
                return ApplicationErrors.WorkOrderIsOccupied;
            }

            if (_workOrderRules.IsTechnicianOccupied(request.EmployeeId , workOrder.StartTime , workOrder.EndTime))
            {
                return ApplicationErrors.TechnicianIsOccupied;
            }

            var updateResult = workOrder.UpdateTechnician(request.EmployeeId);

            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("work-orders", ct);

            return Result.Updated;
        }
    }
}


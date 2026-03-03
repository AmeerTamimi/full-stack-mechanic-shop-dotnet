using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrder
{
    public sealed record class UpdateWorkOrderCommand(
            Guid Id,
            Guid VehicleId,
            Guid CustomerId,
            Guid EmployeeId,
            DateTime StartTime,
            Bay Bay,
            IReadOnlyList<WorkOrderRepairTasksCommand> WorkOrderRepairTasks)
            : IRequest<Result<Updated>>;
}

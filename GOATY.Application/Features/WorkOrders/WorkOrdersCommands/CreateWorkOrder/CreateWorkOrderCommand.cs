using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.CreateWorkOrder
{

    public sealed record class CreateWorkOrderCommand(
            Guid VehicleId,
            Guid CustomerId,
            Guid EmployeeId,
            DateTime StartTime,
            IReadOnlyList<WorkOrderRepairTasksCommand> WorkOrderRepairTasks)
            : IRequest<Result<WorkOrderDto>>;

}

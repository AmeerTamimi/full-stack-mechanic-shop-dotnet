using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands;
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
            Bay Bay,
            IReadOnlyList<WorkOrderRepairTasksCommand> WorkOrderRepairTasks)
            : IRequest<Result<WorkOrderDto>>;

}

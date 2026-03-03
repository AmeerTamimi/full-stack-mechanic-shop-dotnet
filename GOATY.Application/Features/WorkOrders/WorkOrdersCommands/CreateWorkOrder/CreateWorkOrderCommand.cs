using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.CreateWorkOrder
{
    public sealed record class RepairTaskTemplate(Guid Id , TimeStamps Time , decimal Cost);
    public sealed record class CreateWorkOrderCommand(
            Guid VehicleId,
            Guid CustomerId,
            Guid EmployeeId,
            State State,
            DateTime StartTime,
            IReadOnlyList<RepairTaskTemplate> WorkOrderRepairTasks)
            : IRequest<Result<WorkOrderDto>>;

}

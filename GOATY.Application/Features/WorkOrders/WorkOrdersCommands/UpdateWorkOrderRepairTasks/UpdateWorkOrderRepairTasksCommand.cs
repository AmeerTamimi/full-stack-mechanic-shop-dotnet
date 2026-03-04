using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderRepairTasks
{
    public sealed record class UpdateWorkOrderRepairTasksCommand(
        Guid WorkOrderId,
        IReadOnlyList<WorkOrderRepairTasksCommand> WorkOrderRepairTasks)
        : IRequest<Result<Updated>>;
}

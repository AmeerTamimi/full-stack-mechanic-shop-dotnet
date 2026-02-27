using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks.Enums;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.UpdateRepairTaskCommands
{
    public sealed record class UpdateRepairTaskCommand(
        Guid Id,
        string Name,
        string Description,
        TimeEstimations TimeEstimated,
        decimal CostEstimated,
        IReadOnlyList<PartRequirements> Parts) 
        : IRequest<Result<Updated>>;
}

using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.UpdateRepairTaskCommands
{
    public sealed record class UpdateRepairTaskCommand(
        Guid Id,
        string Name,
        string Description,
        decimal TimeEstimated,
        decimal CostEstimated,
        IReadOnlyList<PartRequirements> Parts) 
        : IRequest<Result<Updated>>;
}

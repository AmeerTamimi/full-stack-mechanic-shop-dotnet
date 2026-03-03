using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Enums;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands
{
    public record class PartRequirements(Guid Id, int Quantity);
    public record class CreateRepairTaskCommand(
        string Name,
        string Description,
        TimeStamps TimeEstimated,
        decimal CostEstimated,
        IReadOnlyList<PartRequirements> Parts)
        : IRequest<Result<RepairTaskDto>>;
}


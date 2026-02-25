using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Contracts.Requests;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands
{
    public record class PartRequirements(Guid Id, int Quantity);
    public record class CreateRepairTaskCommand(
        string Name,
        string Description,
        decimal TimeEstimated,
        decimal CostEstimated,
        IReadOnlyList<PartRequirements> Parts)
        : IRequest<Result<RepairTaskDto>>;
}


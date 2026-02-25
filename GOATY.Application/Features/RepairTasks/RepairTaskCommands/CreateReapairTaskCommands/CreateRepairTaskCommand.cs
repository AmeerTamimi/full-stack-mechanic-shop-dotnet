using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Contracts.Requests;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairTasks;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateReapairTaskCommands
{
    public record class CreateRepairTaskCommand(
        string Name,
        string Description,
        decimal TimeEstimated,
        decimal CostEstimated,
        List<KeyValuePair<Guid , int>> parts)
        : IRequest<Result<RepairTaskDto>>;
}

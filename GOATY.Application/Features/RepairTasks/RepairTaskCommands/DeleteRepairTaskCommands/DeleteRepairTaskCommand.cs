using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskCommands.DeleteRepairTaskCommands
{
    public sealed record class DeleteRepairTaskCommand(Guid Id) 
        : IRequest<Result<RepairTaskDto>>;
}

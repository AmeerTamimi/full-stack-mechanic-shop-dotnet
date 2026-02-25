using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTaskById
{
    public record class GetRepairTaskByIdQuery(Guid Id) : IRequest<Result<RepairTaskDto>>;
}

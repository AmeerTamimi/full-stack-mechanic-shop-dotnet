using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery
{
    public record class GetRepairTaskQuery : IRequest<Result<List<RepairTaskDto>>>;
}

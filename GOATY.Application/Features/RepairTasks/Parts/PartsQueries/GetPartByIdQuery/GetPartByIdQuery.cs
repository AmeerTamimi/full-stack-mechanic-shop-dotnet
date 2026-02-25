using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.Parts.PartsQueries.GetPartByIdQuery
{
    public sealed record GetPartByIdQuery(Guid Id) : IRequest<Result<PartDto>>;
}

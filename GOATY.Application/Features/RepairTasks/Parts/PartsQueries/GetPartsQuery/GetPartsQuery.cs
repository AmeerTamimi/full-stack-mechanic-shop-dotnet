using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.Parts.PartsQueries.GetPartsQuery
{
    public sealed record class GetPartsQuery : IRequest<Result<List<PartDto>>>;
}

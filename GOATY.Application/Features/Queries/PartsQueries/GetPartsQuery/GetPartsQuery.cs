using GOATY.Application.Features.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Queries.PartsQueries.GetPartsQuery
{
    public sealed record class GetPartsQuery : IRequest<Result<List<PartDto>>>;
}

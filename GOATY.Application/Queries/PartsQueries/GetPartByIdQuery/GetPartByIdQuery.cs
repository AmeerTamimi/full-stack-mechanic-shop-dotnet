using GOATY.Application.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Queries.PartsQueries.GetPartByIdQuery
{
    public sealed record GetPartByIdQuery(Guid id) : IRequest<Result<PartDto>>;
}

using GOATY.Application.Mapping.PartsMapping;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Queries.PartsQueries.GetPartByIdQuery
{
    public sealed record GetPartByIdQuery(Guid id) : IRequest<Result<PartDto>>;
}

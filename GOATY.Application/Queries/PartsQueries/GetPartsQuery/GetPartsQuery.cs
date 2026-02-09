using GOATY.Application.Mapping.PartsMapping;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Queries.PartsQueries.GetPartsQuery
{
    public sealed record class GetPartsQuery : IRequest<Result<List<PartDto>>>;
}

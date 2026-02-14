using GOATY.Application.Features.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Queries.PartsQueries.GetPartByIdQuery
{
    public sealed record GetPartByIdQuery(Guid Id) : IRequest<Result<PartDto>>;
}

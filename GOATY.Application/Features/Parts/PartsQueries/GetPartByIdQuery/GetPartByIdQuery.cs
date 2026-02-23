using GOATY.Application.Features.Parts.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Parts.PartsQueries.GetPartByIdQuery
{
    public sealed record GetPartByIdQuery(Guid Id) : IRequest<Result<PartDto>>;
}

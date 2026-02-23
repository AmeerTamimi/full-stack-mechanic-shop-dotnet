using GOATY.Application.Features.Parts.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Parts.PartsQueries.GetPartsQuery
{
    public sealed record class GetPartsQuery : IRequest<Result<List<PartDto>>>;
}

using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Parts.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Parts.PartsQueries.GetPartByIdQuery
{
    public sealed record GetPartByIdQuery(Guid Id) : ICachedQuery<Result<PartDto>>
    {
        public string CacheKey => $"parts_{Id}";

        public string[] Tags => ["parts"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}

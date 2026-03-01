using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Parts.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Parts.PartsQueries.GetPartsQuery
{
    public sealed record class GetPartsQuery : ICachedQuery<Result<List<PartDto>>>
    {
        public string CacheKey => "parts";

        public string[] Tags => ["parts"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}

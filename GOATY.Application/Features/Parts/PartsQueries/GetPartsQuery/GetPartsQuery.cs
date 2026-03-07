using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Parts.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Parts.PartsQueries.GetPartsQuery
{
    public sealed record class GetPartsQuery(int Page, int PageSize)
        : ICachedQuery<Result<PaginatedList<PartDto>>>
    {
        public string CacheKey =>
            $"parts:" +
            $"p={Page}:" +
            $"ps={PageSize}";

        public string[] Tags => ["parts"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
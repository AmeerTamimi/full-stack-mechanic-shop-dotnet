using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Parts.DTOs;
using GOATY.Application.Features.Parts.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Parts.PartsQueries.GetPartsQuery
{
    public sealed class GetPartsQueryHandler(
        IAppDbContext context,
        ILogger<GetPartsQueryHandler> logger)
        : IRequestHandler<GetPartsQuery, Result<PaginatedList<PartDto>>>
    {
        public async Task<Result<PaginatedList<PartDto>>> Handle(GetPartsQuery request, CancellationToken ct)
        {
            var partsQuery = context.Parts
                                    .AsNoTracking()
                                    .AsQueryable();

            var count = await partsQuery.CountAsync(ct);

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var partModels = await partsQuery
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync(ct);

            logger.LogInformation("Handled {Query}. Returned {PartsCount} Parts",
                nameof(GetPartsQuery),
                partModels.Count);

            return new PaginatedList<PartDto>
            {
                Items = partModels.ToDtos(),
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }
    }
}
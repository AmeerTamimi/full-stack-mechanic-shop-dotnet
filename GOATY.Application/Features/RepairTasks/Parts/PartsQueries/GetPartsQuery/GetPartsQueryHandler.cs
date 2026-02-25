using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.RepairTasks.Parts.PartsQueries.GetPartsQuery
{
    public sealed class GetPartsQueryHandler(
        IAppDbContext context,
        ILogger<GetPartsQueryHandler> logger

        ) : IRequestHandler<GetPartsQuery, Result<List<PartDto>>>
    {
        public async Task<Result<List<PartDto>>> Handle(GetPartsQuery request, CancellationToken ct)
        {
            logger.LogInformation("Handling {Query}" , nameof(GetPartsQuery));

            var partModels = await context.Parts
                                          .AsNoTracking()
                                          .ToListAsync(ct);

            logger.LogInformation("Handled {Query}. Returned {PartsCount} Parts",
                                                   nameof(GetPartsQuery),
                                                   partModels.Count());

            return partModels.ToDtos();
        }
    }
}

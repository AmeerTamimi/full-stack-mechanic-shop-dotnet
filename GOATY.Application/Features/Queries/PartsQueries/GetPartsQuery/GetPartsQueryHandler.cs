using GOATY.Application.Features.Common;
using GOATY.Application.Features.DTOs;
using GOATY.Application.Features.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Queries.PartsQueries.GetPartsQuery
{
    public sealed class GetPartsQueryHandler(IAppDbContext context) : IRequestHandler<GetPartsQuery, Result<List<PartDto>>>
    {
        public async Task<Result<List<PartDto>>> Handle(GetPartsQuery request, CancellationToken ct)
        {
            var partModels = await context.Parts.ToListAsync(ct);
            return partModels.ToDtos();
        }
    }
}

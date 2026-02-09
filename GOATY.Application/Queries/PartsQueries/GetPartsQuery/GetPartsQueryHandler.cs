using GOATY.Application.Common;
using GOATY.Application.Mapping.PartsMapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Queries.PartsQueries.GetPartsQuery
{
    public sealed class GetPartsQueryHandler(IAppDbContext context) : IRequestHandler<GetPartsQuery, Result<List<PartDto>>>
    {
        public async Task<Result<List<PartDto>>> Handle(GetPartsQuery request, CancellationToken cancellationToken)
        {
            var partModel = await context.Parts.ToListAsync();
            return PartDto.ToDtos(partModel);
        }
    }
}

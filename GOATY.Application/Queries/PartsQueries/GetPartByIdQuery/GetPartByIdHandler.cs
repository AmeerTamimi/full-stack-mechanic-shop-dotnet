using GOATY.Application.Common;
using GOATY.Application.Mapping.PartsMapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Queries.PartsQueries.GetPartByIdQuery
{
    public sealed class GetPartByIdHandler(IAppDbContext context) : IRequestHandler<GetPartByIdQuery, Result<PartDto>>
    {
        public async Task<Result<PartDto>> Handle(GetPartByIdQuery request, CancellationToken cancellationToken)
        {
            var partModel = await context.Parts.SingleOrDefaultAsync(p => p.Id == request.id);
            return PartDto.ToDto(partModel!);
        }
    }
}

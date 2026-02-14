using GOATY.Application.Common;
using GOATY.Application.DTOs;
using GOATY.Application.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Queries.PartsQueries.GetPartByIdQuery
{
    public sealed class GetPartByIdHandler(IAppDbContext context) : IRequestHandler<GetPartByIdQuery, Result<PartDto>>
    {
        public async Task<Result<PartDto>> Handle(GetPartByIdQuery request, CancellationToken ct)
        {
            var id = request.Id;

            var partModel = await context.Parts.SingleOrDefaultAsync(p => p.Id == id , ct);

            if(partModel is null)
            {
                return Error.NotFound( // this return Result<T>(error) -> T : partDtp , error : the error we made
                    code: "Part_NotFound",
                    description: $"Part With Id {id} was Not Found"
                );
            }
            return partModel.ToDto();
        }
    }
}

using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace GOATY.Application.Features.Parts.PartsCommands.UpdatePartCommands
{
    public sealed class UpdatePartCommandHandler(IAppDbContext context , HybridCache cache) : IRequestHandler<UpdatePartCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdatePartCommand request, CancellationToken ct)
        {
            var id = request.Id;
            var part = await context.Parts.SingleOrDefaultAsync(
                                                   p => p.Id == id,
                                                   ct);

            if(part is null)
            {
                return Error.NotFound(
                             code: "Part_NotFound",
                             description: $"Part With Id {id} was not Found."
                            );
            }

            var updateResult = part.Update(request.Name,
                                           request.Cost,
                                           request.Quantity);

            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("parts", ct);

            return Result.Updated;
        }
    }
}

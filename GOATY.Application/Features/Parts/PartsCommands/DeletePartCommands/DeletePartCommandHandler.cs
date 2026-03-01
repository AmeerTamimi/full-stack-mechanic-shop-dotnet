using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace GOATY.Application.Features.Parts.PartsCommands.DeletePartCommands
{
    public sealed class DeletePartCommandHandler(IAppDbContext context , HybridCache cache) : IRequestHandler<DeletePartCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(DeletePartCommand request, CancellationToken ct)
        {
            var id = request.Id;
            var partToDelete = await context.Parts.SingleOrDefaultAsync(
                                                   p => p.Id == id,
                                                   ct);

            if(partToDelete is null)
            {
                // log
                return Error.NotFound(
                             code: "Part_NotFound",
                             description: $"Part With Id {id} was not Found."
                            );
            }

            context.Parts.Remove(partToDelete);

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("parts", ct);

            return id;
        }
    }
}

using GOATY.Application.Features.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Parts.PartsCommands.UpdatePartCommands
{
    public sealed class UpdatePartCommandHandler(IAppDbContext context) : IRequestHandler<UpdatePartCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdatePartCommand request, CancellationToken ct)
        {
            var id = request.Id;
            var partToUpdate = await context.Parts.SingleOrDefaultAsync(
                                                   p => p.Id == id,
                                                   ct);

            if(partToUpdate is null)
            {
                return Error.NotFound(
                             code: "Part_NotFound",
                             description: $"Part With Id {id} was not Found."
                            );
            }

            var updateResult = Part.Update(partToUpdate,
                                           request.Name,
                                           request.Cost,
                                           request.Quantity
                                        );

            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            return Result.Updated;
        }
    }
}

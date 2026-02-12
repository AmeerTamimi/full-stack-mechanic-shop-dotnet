using GOATY.Application.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Commands.PartsCommands.UpdatePartCommands
{
    public sealed class UpdatePartCommandHandler(IAppDbContext context) : IRequestHandler<UpdatePartCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdatePartCommand request, CancellationToken ct)
        {
            var id = request.id;
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
                                           request.name,
                                           request.cost,
                                           request.quantity
                                        );


            await context.SaveChangesAsync(ct);

            return Result.Updated;
        }
    }
}

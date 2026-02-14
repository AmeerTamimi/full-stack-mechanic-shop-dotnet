using GOATY.Application.Features.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Commands.PartsCommands.DeletePartCommands
{
    public sealed class DeletePartCommandHandler(IAppDbContext context) : IRequestHandler<DeletePartCommand, Result<Guid>>
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

            return id;
        }
    }
}

using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Application.Features.RepairTasks.Mapping;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RepairsTask.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.RepairTasks.Parts.PartsCommands.CreatePartCommands
{
    public sealed class CreatePartCommandHandler(IAppDbContext context) : IRequestHandler<CreatePartCommand, Result<PartDto>>
    {
        public async Task<Result<PartDto>> Handle(CreatePartCommand request, CancellationToken ct)
        {
            var name = request.Name;
            var result = Part.Create(Guid.NewGuid(), name, request.Cost, request.Quantity);

            if (!result.IsSuccess)
            {
                return result.Errors;
            }

            var nameExists = await context.Parts.AnyAsync(p => p.Name == name , ct);

            if (nameExists)
            {
                // warning log
                return Error.Conflict(
                                code: "Part.Name.Conflict",
                                description: $"The Part Name {name} Already Exists."
                            );
            }

            var newPart = result.Value;

            await context.Parts.AddAsync(newPart);
            await context.SaveChangesAsync(ct);

            return newPart.ToDto();
        }
    }
}

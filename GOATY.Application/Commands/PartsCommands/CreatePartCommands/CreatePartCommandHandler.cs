using GOATY.Application.Common;
using GOATY.Application.DTOs;
using GOATY.Application.Mapping;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Commands.PartsCommands.CreatePartCommands
{
    public sealed class CreatePartCommandHandler(IAppDbContext context) : IRequestHandler<CreatePartCommand, Result<PartDto>>
    {
        public async Task<Result<PartDto>> Handle(CreatePartCommand request, CancellationToken ct)
        {
            var name = request.name;
            var cost = request.cost;
            var quantity = request.quantity;

            var nameExists = await context.Parts.AnyAsync(p => p.Name == name , ct);

            if (nameExists)
            {
                // warning log
                return PartErrors.NameInvalidError;
            }

            var result = Part.Create(Guid.NewGuid(), name, cost, quantity);

            if (!result.IsSuccess)
            {
                Console.WriteLine("Helooo");
                return result.Errors;
            }

            var newPart = result.Value;

            await context.Parts.AddAsync(newPart);
            await context.SaveChangesAsync(ct);

            return newPart.ToDto();
        }
    }
}

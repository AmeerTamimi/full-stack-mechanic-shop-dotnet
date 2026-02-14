using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Commands.PartsCommands.UpdatePartCommands
{
    public sealed record class UpdatePartCommand(
        Guid Id,
        string Name,
        decimal Cost,
        int Quantity) 
        : IRequest<Result<Updated>>;
}

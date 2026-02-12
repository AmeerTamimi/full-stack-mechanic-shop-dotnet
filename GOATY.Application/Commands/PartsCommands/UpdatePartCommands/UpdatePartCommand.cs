using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Commands.PartsCommands.UpdatePartCommands
{
    public sealed record class UpdatePartCommand(
        Guid id,
        string name,
        decimal cost,
        int quantity) 
        : IRequest<Result<Updated>>;
}

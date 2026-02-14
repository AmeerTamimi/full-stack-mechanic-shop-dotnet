using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Commands.PartsCommands.DeletePartCommands
{
    public sealed record class DeletePartCommand(Guid Id) : IRequest<Result<Guid>>;
}

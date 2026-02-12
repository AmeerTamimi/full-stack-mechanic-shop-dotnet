using GOATY.Application.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Commands.PartsCommands.CreatePartCommands
{
    public sealed record class CreatePartCommand(
        string name,
        decimal cost,
        int quantity)
        : IRequest<Result<PartDto>>;
}

using GOATY.Application.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Commands.PartsCommands.CreatePartCommands
{
    public sealed record class CreatePartCommand(
        string Name,
        decimal Cost,
        int Quantity)
        : IRequest<Result<PartDto>>;
}

using GOATY.Application.DTOs;
using MediatR;

namespace GOATY.Application.Commands.PartsCommands.CreatePartCommands
{
    public sealed record class CreatePartCommand(decimal cost , string name , int quantity) : IRequest<PartDto>;
}

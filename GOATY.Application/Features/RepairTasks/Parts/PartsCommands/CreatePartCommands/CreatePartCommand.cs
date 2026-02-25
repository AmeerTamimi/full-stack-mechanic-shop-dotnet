using GOATY.Application.Features.RepairTasks.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.RepairTasks.Parts.PartsCommands.CreatePartCommands
{
    public sealed record class CreatePartCommand(
        string Name,
        decimal Cost,
        int Quantity)
        : IRequest<Result<PartDto>>;
}

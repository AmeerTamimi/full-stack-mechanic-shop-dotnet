using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Commands.EmployeeCommands.DeleteEmployeeCommand
{
    public sealed record class DeleteEmployeeCommand(Guid Id) : IRequest<Result<Deleted>>;
}

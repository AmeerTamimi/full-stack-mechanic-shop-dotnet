using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Employees.EmployeeCommands.DeleteEmployeeCommand
{
    public sealed record class DeleteEmployeeCommand(Guid Id) : IRequest<Result<Deleted>>;
}

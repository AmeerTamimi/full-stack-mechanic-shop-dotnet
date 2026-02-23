using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees.Enums;
using MediatR;

namespace GOATY.Application.Features.Employees.EmployeeCommands.UpdateEmployeeCommand
{
    public sealed record class UpdateEmployeeCommand (
            Guid Id,
            string FirstName,
            string LastName,
            Role Role
        ) : IRequest<Result<Updated>>;
}

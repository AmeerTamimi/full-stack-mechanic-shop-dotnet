using GOATY.Application.Features.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees.Enums;
using MediatR;

namespace GOATY.Application.Features.Commands.EmployeeCommands.CreateEmployeeCommand
{
    public sealed record class CreateEmployeeCommand(
            string FirstName,
            string LastName,
            Role Role
        ) : IRequest<Result<EmployeeDto>>;
}

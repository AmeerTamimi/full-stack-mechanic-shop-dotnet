using GOATY.Application.Features.Common;
using GOATY.Application.Features.DTOs;
using GOATY.Application.Features.Mapping;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using MediatR;

namespace GOATY.Application.Features.Commands.EmployeeCommands.CreateEmployeeCommand
{
    public sealed class CreateEmployeeCommandHandler(IAppDbContext context) : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
    {
        public async Task<Result<EmployeeDto>> Handle(CreateEmployeeCommand request, CancellationToken ct)
        {
            var result = Employee.Create(Guid.NewGuid(),  
                                         request.FirstName,
                                         request.LastName,
                                         request.Role
                                  );
            
            if (!result.IsSuccess)
            {
                return result.Errors;
            }

            var employee= result.Value;

            await context.Employees.AddAsync(employee, ct);
            await context.SaveChangesAsync(ct);

            return employee.ToDto();
        }
    }
}

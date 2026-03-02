using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Employees.DTOs;
using GOATY.Application.Features.Employees.Mapping;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace GOATY.Application.Features.Employees.EmployeeCommands.CreateEmployeeCommand
{
    public sealed class CreateEmployeeCommandHandler(IAppDbContext context , HybridCache cache) : IRequestHandler<CreateEmployeeCommand, Result<EmployeeDto>>
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

            await cache.RemoveByTagAsync("employees");

            return employee.ToDto();
        }
    }
}

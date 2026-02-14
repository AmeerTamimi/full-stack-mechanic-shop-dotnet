using GOATY.Application.Features.Common;
using GOATY.Application.Features.DTOs;
using GOATY.Application.Features.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Queries.EmployeeQueries.GetEmployeeByIdQuery
{
    public sealed class GetEmployeeByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
    {
        public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
        {
            var employeeId = request.Id;

            var employee = await context.Employees.AsNoTracking()
                                                  .SingleOrDefaultAsync(
                                                        emp => emp.Id == employeeId , ct
                                                  );

            if(employee is null)
            {
                return Error.NotFound(
                                code : "Employee.NotFound",
                                description : $"Employee With Id {employeeId} Was Not Found."
                            );
            }

            return employee.ToDto();
        }
    }
}

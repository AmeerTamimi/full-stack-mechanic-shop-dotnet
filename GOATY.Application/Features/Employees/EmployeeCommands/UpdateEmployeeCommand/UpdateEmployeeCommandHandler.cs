using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace GOATY.Application.Features.Employees.EmployeeCommands.UpdateEmployeeCommand
{
    public sealed class UpdateEmployeeCommandHandler(IAppDbContext context , HybridCache cache) : IRequestHandler<UpdateEmployeeCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateEmployeeCommand request, CancellationToken ct)
        {
            var employeeId = request.Id;

            var employee = await context.Employees.SingleOrDefaultAsync(
                                                            emp => emp.Id == employeeId,
                                                            ct
                                                          );

            if(employee is null)
            {
                return Error.NotFound(
                                code: "Employee.NotFound",
                                description: $"Employee With Id {employeeId} Was Not Found."
                            );
            }

            var result = Employee.Update(employee,
                                         request.FirstName,
                                         request.LastName,
                                         request.Role
                                  );

            if (!result.IsSuccess)
            {
                return result.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("employees");

            return Result.Updated;
        }
    }
}

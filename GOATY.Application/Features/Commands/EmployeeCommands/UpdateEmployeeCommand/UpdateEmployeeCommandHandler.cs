using GOATY.Application.Features.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Commands.EmployeeCommands.UpdateEmployeeCommand
{
    public sealed class UpdateEmployeeCommandHandler(IAppDbContext context) : IRequestHandler<UpdateEmployeeCommand, Result<Updated>>
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

            return Result.Updated;
        }
    }
}

using GOATY.Application.Features.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Commands.EmployeeCommands.DeleteEmployeeCommand
{
    public sealed class DeleteEmployeeCommandHandler(IAppDbContext context) : IRequestHandler<DeleteEmployeeCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteEmployeeCommand request, CancellationToken ct)
        {
            var employeeId = request.Id;
            var employee = await context.Employees.SingleOrDefaultAsync(
                                                        emp => emp.Id == employeeId,
                                                        ct
                                                   );

            if (employee is null)
            {
                return Error.NotFound(
                                code: "Employee.NotFound",
                                description: $"Employee With Id {employeeId} Was Not Found."
                            );
            }

            context.Employees.Remove(employee);
            await context.SaveChangesAsync(ct);

            return Result.Deleted;
        }
    }
}

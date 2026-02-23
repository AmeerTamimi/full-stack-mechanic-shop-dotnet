using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.Employees.DTOs;
using GOATY.Application.Features.Employees.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeesQuery
{
    public sealed class GetEmployeesQueryHandler(
        IAppDbContext context,
        ILogger<GetEmployeesQueryHandler> logger
        ) : IRequestHandler<GetEmployeesQuery, Result<List<EmployeeDto>>>
    {
        public async Task<Result<List<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken ct)
        {
            logger.LogInformation("Handling {Query}", nameof(GetEmployeesQuery));

            var employees =  await context.Employees
                                          .AsNoTracking()
                                          .ToListAsync(ct);

            logger.LogInformation("Handled {Query}. Returned {EmployeeCount} Employees",
                                                                    nameof(GetEmployeesQuery),
                                                                    employees.Count()
                                                                );
            return employees.ToDtos();
        }
    }
}

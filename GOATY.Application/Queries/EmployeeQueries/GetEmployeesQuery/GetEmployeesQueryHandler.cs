using GOATY.Application.Common;
using GOATY.Application.DTOs;
using GOATY.Application.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Queries.EmployeeQueries.GetEmployeesQuery
{
    public sealed class GetEmployeesQueryHandler(IAppDbContext context) : IRequestHandler<GetEmployeesQuery, Result<List<EmployeeDto>>>
    {
        public async Task<Result<List<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken ct)
        {
            var employees =  await context.Employees.ToListAsync(ct);

            return employees.ToDtos();
        }
    }
}

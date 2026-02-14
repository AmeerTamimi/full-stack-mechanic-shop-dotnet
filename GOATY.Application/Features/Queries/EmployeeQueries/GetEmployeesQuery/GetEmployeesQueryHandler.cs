using GOATY.Application.Features.Common;
using GOATY.Application.Features.DTOs;
using GOATY.Application.Features.Mapping;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Queries.EmployeeQueries.GetEmployeesQuery
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

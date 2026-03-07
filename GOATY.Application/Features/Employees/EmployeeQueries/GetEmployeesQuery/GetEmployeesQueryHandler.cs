using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
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
        ) : IRequestHandler<GetEmployeesQuery, Result<PaginatedList<EmployeeDto>>>
    {
        public async Task<Result<PaginatedList<EmployeeDto>>> Handle(GetEmployeesQuery request, CancellationToken ct)
        {
            logger.LogInformation("Handling {Query}", nameof(GetEmployeesQuery));

            var employeesQuery = context.Employees
                                        .AsNoTracking()
                                        .AsQueryable();

            var count = await employeesQuery.CountAsync(ct);

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var employees = await employeesQuery
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync(ct);

            logger.LogInformation("Handled {Query}. Returned {EmployeeCount} Employees",
                nameof(GetEmployeesQuery),
                employees.Count);

            return new PaginatedList<EmployeeDto>
            {
                Items = employees.ToDtos(),
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }
    }
}
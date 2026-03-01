using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeesQuery
{
    public record class GetEmployeesQuery : ICachedQuery<Result<List<EmployeeDto>>>
    {
        public string CacheKey => "employees";

        public string[] Tags => ["employees"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}

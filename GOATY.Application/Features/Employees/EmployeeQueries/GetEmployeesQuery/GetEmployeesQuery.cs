using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeesQuery
{
    public sealed record class GetEmployeesQuery(int Page, int PageSize)
        : ICachedQuery<Result<PaginatedList<EmployeeDto>>>
    {
        public string CacheKey =>
            $"employees:" +
            $"p={Page}:" +
            $"ps={PageSize}";

        public string[] Tags => ["employees"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
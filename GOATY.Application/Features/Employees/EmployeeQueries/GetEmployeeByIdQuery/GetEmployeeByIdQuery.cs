using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeeByIdQuery
{
    public sealed record class GetEmployeeByIdQuery(Guid Id) : ICachedQuery<Result<EmployeeDto>>
    {
        public string CacheKey => $"employees_{Id}";

        public string[] Tags => ["employees"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}

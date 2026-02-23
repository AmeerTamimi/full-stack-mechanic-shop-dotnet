using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeeByIdQuery
{
    public sealed record class GetEmployeeByIdQuery(Guid Id) : IRequest<Result<EmployeeDto>>;
}

using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeesQuery
{
    public record class GetEmployeesQuery : IRequest<Result<List<EmployeeDto>>>;
    
}

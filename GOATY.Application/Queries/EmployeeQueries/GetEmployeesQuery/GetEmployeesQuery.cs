using GOATY.Application.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Queries.EmployeeQueries.GetEmployeesQuery
{
    public record class GetEmployeesQuery : IRequest<Result<List<EmployeeDto>>>;
    
}

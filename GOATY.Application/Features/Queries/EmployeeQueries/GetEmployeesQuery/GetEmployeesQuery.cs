using GOATY.Application.Features.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Queries.EmployeeQueries.GetEmployeesQuery
{
    public record class GetEmployeesQuery : IRequest<Result<List<EmployeeDto>>>;
    
}

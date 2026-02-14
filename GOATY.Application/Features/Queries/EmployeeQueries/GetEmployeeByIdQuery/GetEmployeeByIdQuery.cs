using GOATY.Application.Features.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Queries.EmployeeQueries.GetEmployeeByIdQuery
{
    public sealed record class GetEmployeeByIdQuery(Guid Id) : IRequest<Result<EmployeeDto>>;
}

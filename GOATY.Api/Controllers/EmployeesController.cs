using GOATY.Application.Features.Employees.EmployeeCommands.CreateEmployeeCommand;
using GOATY.Application.Features.Employees.EmployeeCommands.DeleteEmployeeCommand;
using GOATY.Application.Features.Employees.EmployeeCommands.UpdateEmployeeCommand;
using GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeeByIdQuery;
using GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeesQuery;
using GOATY.Contracts.Requests;
using GOATY.Domain.Employees.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    //[Authorize(Roles = nameof(Role.Manager))]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetEmployees(PaginationRequest request)
        {
            var result = await mediator.Send(new GetEmployeesQuery(request.Page , request.PageSize));

            return result.Match(
                response => Ok(response),
                Problem
                );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var result = await mediator.Send(new GetEmployeeByIdQuery(id));

            return result.Match(
                response => Ok(response),
                Problem
                );
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest request)
        {
            var result = await mediator.Send(new CreateEmployeeCommand(request.FirstName! , request.LastName! , request.Role));

            return result.Match(
                response => Ok(response),
                Problem
                );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeRequest request)
        {
            var result = await mediator.Send(new UpdateEmployeeCommand(id , request.FirstName!, request.LastName!, request.Role));

            return result.Match(
                response => NoContent(),
                Problem
                );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var result = await mediator.Send(new DeleteEmployeeCommand(id));

            return result.Match(
                response => Ok(id),
                Problem
                );
        }
    }
}

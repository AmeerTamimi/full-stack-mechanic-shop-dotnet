using GOATY.Application.Common.Models;
using GOATY.Application.Features.Employees.DTOs;
using GOATY.Application.Features.Employees.EmployeeCommands.CreateEmployeeCommand;
using GOATY.Application.Features.Employees.EmployeeCommands.DeleteEmployeeCommand;
using GOATY.Application.Features.Employees.EmployeeCommands.UpdateEmployeeCommand;
using GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeeByIdQuery;
using GOATY.Application.Features.Employees.EmployeeQueries.GetEmployeesQuery;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Contracts.Requests;
using GOATY.Domain.Employees.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Authorize(Roles = nameof(Role.Manager))]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IMediator mediator) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetEmployees")]
        [EndpointSummary("Retrieves a paginated list of employees.")]
        [EndpointDescription("Returns a paginated list of employees in the system. Only users with the Manager role are authorized to access this resource.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetEmployees([FromQuery] PaginationRequest request)
        {
            var result = await mediator.Send(new GetEmployeesQuery(request.Page, request.PageSize));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetEmployeeById")]
        [EndpointSummary("Retrieves an employee by its ID.")]
        [EndpointDescription("Returns detailed information about the specified employee if it exists.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var result = await mediator.Send(new GetEmployeeByIdQuery(id));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateEmployee")]
        [EndpointSummary("Creates a new employee.")]
        [EndpointDescription("Creates a new employee record with the specified first name, last name, and role. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequest request)
        {
            var result = await mediator.Send(new CreateEmployeeCommand(request.FirstName!, request.LastName!, request.Role));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateEmployee")]
        [EndpointSummary("Updates an existing employee.")]
        [EndpointDescription("Updates the specified employee record including first name, last name, and role. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeRequest request)
        {
            var result = await mediator.Send(new UpdateEmployeeCommand(id, request.FirstName!, request.LastName!, request.Role));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteEmployee")]
        [EndpointSummary("Deletes an employee.")]
        [EndpointDescription("Deletes the specified employee record from the system. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
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
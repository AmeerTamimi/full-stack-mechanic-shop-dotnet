using GOATY.Application.Features.Customers.CustomerCommands;
using GOATY.Application.Features.Customers.CustomerCommands.CreateCustomer;
using GOATY.Application.Features.Customers.CustomerCommands.DeleteCustomer;
using GOATY.Application.Features.Customers.CustomerCommands.UpdateCustomer;
using GOATY.Application.Features.Customers.CustomerQueries.GetCustomerById;
using GOATY.Application.Features.Customers.CustomerQueries.GetCustomers;
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
    public sealed class CustomersController(IMediator mediator) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetCustomers")]
        [EndpointSummary("Retrieves a paginated list of customers.")]
        [EndpointDescription("Returns a paginated list of customers in the system. Only users with the Manager role are authorized to access this resource.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCustomers([FromQuery] PaginationRequest request)
        {
            var result = await mediator.Send(new GetCustomersQuery(request.Page, request.PageSize));

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetCustomerById")]
        [EndpointSummary("Retrieves a customer by its ID.")]
        [EndpointDescription("Returns detailed information about the specified customer, including related vehicles, if the customer exists.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var result = await mediator.Send(new GetCustomerByIdQuery(id));

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateCustomer")]
        [EndpointSummary("Creates a new customer.")]
        [EndpointDescription("Creates a new customer record including the associated vehicles. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddCustomer(CustomerRequest request)
        {
            var result = await mediator.Send(
                new CreateCustomerCommand(
                    request.FirstName!,
                    request.LastName!,
                    request.Phone!,
                    request.Email!,
                    request.Address!,
                    request.Vehicles
                    .Select(v => new CreateVehicleCommand(v.Brand!,v.Model!,v.Year,v.LicensePlate!))
                    .ToList()
                )
            );

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateCustomer")]
        [EndpointSummary("Updates an existing customer.")]
        [EndpointDescription("Updates the specified customer record including personal information and related vehicles. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateCustomer(Guid id, CustomerRequest request)
        {
            var result = await mediator.Send(
                new UpdateCustomerCommand(
                    id,
                    request.FirstName!,
                    request.LastName!,
                    request.Phone!,
                    request.Email!,
                    request.Address!,
                    request.Vehicles
                    .Select(v => new UpdateVehicleCommand( v.Id,v.Brand!,v.Model!,v.Year,v.LicensePlate!))
                    .ToList()
                )
            );

            return result.Match<IActionResult>(
                response => NoContent(),
                Problem
            );
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteCustomer")]
        [EndpointSummary("Deletes a customer.")]
        [EndpointDescription("Deletes the specified customer record from the system. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var result = await mediator.Send(new DeleteCustomerCommand(id));

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }
    }
}
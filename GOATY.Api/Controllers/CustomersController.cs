using GOATY.Application.Features.Customers.CustomerCommands;
using GOATY.Application.Features.Customers.CustomerCommands.CreateCustomer;
using GOATY.Application.Features.Customers.CustomerCommands.DeleteCustomer;
using GOATY.Application.Features.Customers.CustomerCommands.UpdateCustomer;
using GOATY.Application.Features.Customers.CustomerQueries.GetCustomerById;
using GOATY.Application.Features.Customers.CustomerQueries.GetCustomers;
using GOATY.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class CustomersController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var result = await mediator.Send(new GetCustomersQuery());

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var result = await mediator.Send(new GetCustomerByIdQuery(id));

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost]
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
                           .Select(v => new CreateVehicleCommand(
                               v.Brand!,
                               v.Model!,
                               v.Year,
                               v.LicensePlate!
                           ))
                           .ToList()
                )
            );

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpPut("{id:guid}")]
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
                           .Select(v => new UpdateVehicleCommand(
                               v.Id,
                               v.Brand!,
                               v.Model!,
                               v.Year,
                               v.LicensePlate!
                           ))
                           .ToList()
                )
            );

            return result.Match<IActionResult>(
                response => NoContent(),
                Problem
            );
        }

        [HttpDelete("{id:guid}")]
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
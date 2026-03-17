using GOATY.Application.Features.Parts.PartsCommands.CreatePartCommands;
using GOATY.Application.Features.Parts.PartsCommands.DeletePartCommands;
using GOATY.Application.Features.Parts.PartsCommands.UpdatePartCommands;
using GOATY.Application.Features.Parts.PartsQueries.GetPartByIdQuery;
using GOATY.Application.Features.Parts.PartsQueries.GetPartsQuery;
using GOATY.Contracts.Requests;
using GOATY.Domain.Employees.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    //[Authorize(Roles = nameof(Role.Manager))]
    [Route("api/parts")]
    [ApiController]
    public class PartsController(IMediator mediator) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetParts")]
        [EndpointSummary("Retrieves a paginated list of parts.")]
        [EndpointDescription("Returns a paginated list of parts available in the inventory. Only users with the Manager role are authorized to access this resource.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetParts([FromQuery] PaginationRequest request)
        {
            var result = await mediator.Send(new GetPartsQuery(request.Page, request.PageSize));

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("{id:guid}" , Name = "GetPartById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetPartById")]
        [EndpointSummary("Retrieves a part by its ID.")]
        [EndpointDescription("Returns detailed information about the specified part, including its cost and available quantity.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetPartById(Guid id)
        {
            var result = await mediator.Send(new GetPartByIdQuery(id));

            return result.Match<IActionResult>(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreatePart")]
        [EndpointSummary("Creates a new part.")]
        [EndpointDescription("Adds a new part to the inventory with its name, cost, and quantity. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddPart([FromBody] PartRequest part)
        {
            var result = await mediator.Send(new CreatePartCommand(part.Name!, part.Cost, part.Quantity));

            return result.Match(
            response => CreatedAtRoute(
                routeName: "GetPartById",
                routeValues: new { version = "1.0", id = response.Id },
                value: response),
            Problem);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdatePart")]
        [EndpointSummary("Updates an existing part.")]
        [EndpointDescription("Updates the specified part including its name, cost, and available quantity. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdatePart(Guid id, [FromBody] PartRequest part)
        {
            var result = await mediator.Send(new UpdatePartCommand(id, part.Name!, part.Cost, part.Quantity));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeletePart")]
        [EndpointSummary("Deletes a part.")]
        [EndpointDescription("Deletes the specified part from the inventory. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeletePart(Guid id)
        {
            var result = await mediator.Send(new DeletePartCommand(id));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}
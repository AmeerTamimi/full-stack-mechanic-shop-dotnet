using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands;
using GOATY.Application.Features.RepairTasks.RepairTaskCommands.DeleteRepairTaskCommands;
using GOATY.Application.Features.RepairTasks.RepairTaskCommands.UpdateRepairTaskCommands;
using GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTaskById;
using GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery;
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
    public class RepairTasksController(IMediator mediator) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetRepairTasks")]
        [EndpointSummary("Retrieves a paginated list of repair tasks.")]
        [EndpointDescription("Returns repair task templates used when creating work orders. Only managers are authorized to access this resource.")]
        [MapToApiVersion("1.0")]

        public async Task<IActionResult> GetRepairTasks([FromQuery] PaginationRequest request)
        {
            var result = await mediator.Send(new GetRepairTaskQuery(request.Page , request.PageSize));

            return result.Match<IActionResult>(
                    response => Ok(response),
                    Problem
                );
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetRepairTaskById")]
        [EndpointSummary("Retrieves a repair task by its ID.")]
        [EndpointDescription("Returns the repair task template including estimated time, cost, and required parts.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetRepairTaskById(Guid id)
        {
            var result = await mediator.Send(new GetRepairTaskByIdQuery(id));

            Console.WriteLine(result.Errors);
            return result.Match<IActionResult>(
                    response => Ok(response),
                    Problem
                );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateRepairTask")]
        [EndpointSummary("Creates a new repair task template.")]
        [EndpointDescription("Creates a repair task template that can later be attached to work orders. Includes estimated time, technician cost, and required parts.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddRepairTask(RepairTaskRequest request)
        {
           var result = await mediator.Send(new CreateRepairTaskCommand(
                request.Name!,
                request.Description!,
                request.TimeEstimated,
                request.CostEstimated,
                request.TechnicianCost,
                request.Parts
                .Select(p => new PartRequirements(p.PartId,p.Quantity))
                .ToList()));

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
        [EndpointName("UpdateRepairTask")]
        [EndpointSummary("Updates an existing repair task.")]
        [EndpointDescription("Updates the repair task template including estimated time, technician cost, and required parts.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateRepairTask(Guid id , RepairTaskRequest request)
        {
            var result = await mediator.Send(new UpdateRepairTaskCommand(
                id,
                request.Name!,
                request.Description!,
                request.TimeEstimated,
                request.CostEstimated,
                request.TechnicianCost,
                request.Parts
                .Select(p => new PartRequirements(p.PartId, p.Quantity))
                .ToList()));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteRepairTask")]
        [EndpointSummary("Deletes a repair task template.")]
        [EndpointDescription("Permanently deletes the specified repair task template.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteRepairTask(Guid id)
        {
            var result = await mediator.Send(new DeleteRepairTaskCommand(id));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}

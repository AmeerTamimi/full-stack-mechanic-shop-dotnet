using Azure.Core;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Schedule.DTOs;
using GOATY.Application.Features.Schedule.Queries.GetSchedule;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.AssignTechnician;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.CreateWorkOrder;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.DeleteWorkOrder;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.RelocateWorkOrder;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateVehicle;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderRepairTasks;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrderState;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands;
using GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrderById;
using GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrders;
using GOATY.Contracts.Requests;
using GOATY.Domain.Employees.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController(IMediator mediator) : ApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedList<WorkOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves a paginated list of work orders.")]
        [EndpointDescription("Supports filtering by date range, status, vehicle, labor, spot, and searching by term. Pagination and sorting are supported.")]
        [EndpointName("GetWorkOrders")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetWorkOrders([FromQuery] GetWorkOrdersRequest query)
        {
            var result = await mediator.Send(new GetWorkOrdersQuery(
                query.Page,
                query.PageSize,
                query.SearchTerm,
                query.SortColumn,
                query.SortDirection,
                query.State,
                query.VehicleId,
                query.LaborId,
                query.StartDateFrom,
                query.StartDateTo,
                query.EndDateFrom,
                query.EndDateTo,
                query.Bay
            ));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("{workOrderId:guid}")]
        [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("GetWorkOrderById")]
        [EndpointSummary("Retrieves a work order by its ID.")]
        [EndpointDescription("Returns detailed information about the specified work order if it exists.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetWorkOrderById(Guid workOrderId)
        {
            var result = await mediator.Send(new GetWorkOrderByIdQuery(workOrderId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Manager))]
        [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("CreateWorkOrder")]
        [EndpointSummary("Creates a new work order.")]
        [EndpointDescription("Creates a new work order for a vehicle, specifying labor, tasks, and other required information.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddWorkOrder(CreateWorkOrderRequest request)
        {
            var result = await mediator.Send(new CreateWorkOrderCommand(
                request.VehicleId,
                request.CustomerId,
                request.EmployeeId,
                request.StartTime,
                request.Bay,
                request.Discount,
                request.Quantity,
                request.WorkOrderRepairTasks
                .Select(wr => new WorkOrderRepairTasksCommand(wr.RepairTaskId))
                .ToList()));

            return result.Match(
            response => CreatedAtRoute(
                routeName: "GetWorkOrderById",
                routeValues: new { version = "1.0", workOrderId = response.Id },
                value: response),
            Problem);
        }

        [HttpPut("{workOrderId:guid}/technician")]
        [Authorize(Roles = nameof(Role.Manager))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("AssignTechnicianToWorkOrder")]
        [EndpointSummary("Assigns a technician to a work order.")]
        [EndpointDescription("Associates a technician definition with a specific work order. Only managers can perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AssignWorkOrderTechnician(Guid workOrderId, AssignTechnicianRequest request)
        {
            var result = await mediator.Send(new AssignTechnicianCommand(workOrderId, request.EmployeeId));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }
        [HttpPut("{workOrderId:guid}/relocation")]
        [Authorize(Roles = nameof(Role.Manager))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("RescheduleWorkOrder")]
        [EndpointSummary("Relocates a work order to a new time and spot.")]
        [EndpointDescription("Updates the scheduled time and assigned bay for a work order. Only users with the Manager role can perform this action.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RelocateWorkOrder([FromRoute] Guid workOrderId, RelocateWorkOrderRequest request)
        {
            var result = await mediator.Send(new RelocateWorkOrderCommand(
                workOrderId,
                request.StartTime,
                request.Bay));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("{workOrderId:guid}/vehicle")]
        [Authorize(Roles = nameof(Role.Manager))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateWorkOrderVehicle")]
        [EndpointSummary("Updates the vehicle associated with a work order.")]
        [EndpointDescription("Changes the vehicle assigned to the specified work order. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateWorkOrderVehicle(Guid workOrderId, UpdateWorkOrderVehicleRequest request)
        {
            var result = await mediator.Send(new UpdateWorkOrderVehicleCommand(workOrderId, request.VehicleId));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("{workOrderId:guid}/state")]
        [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Technician)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateWorkOrderState")]
        [EndpointSummary("Changes the state of a work order.")]
        [EndpointDescription("Updates the current state of the specified work order. Only users with the Manager role are authorized.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateWorkOrderState(Guid workOrderId, UpdateWorkOrderStateRequest request)
        {
            var result = await mediator.Send(new UpdateWorkOrderStateCommand(workOrderId,request.State));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("{workOrderId:guid}/repair-tasks")]
        [Authorize(Roles = nameof(Role.Manager))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("UpdateWorkOrderRepairTasks")]
        [EndpointSummary("Updates the repair tasks associated with a work order.")]
        [EndpointDescription("Replaces the repair task list for the specified work order. Only users with the Manager role are authorized to perform this operation.")]
        [MapToApiVersion("1.0")]

        public async Task<IActionResult> UpdateWorkOrderRepairTasks(Guid workOrderId, UpdateWorkOrderRepairTasksRequest request)
        {
            var result = await mediator.Send(new UpdateWorkOrderRepairTasksCommand(
                workOrderId, 
                request.WorkOrderRepairTasks
                       .Select(wr => new WorkOrderRepairTasksCommand(wr.RepairTaskId)).ToList()));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpDelete("{workOrderId:guid}")]
        [Authorize(Roles = nameof(Role.Manager))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointName("DeleteWorkOrder")]
        [EndpointSummary("Deletes a work order.")]
        [EndpointDescription("Deletes the specified work order permanently. Only users with the Manager role are authorized.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteWorkOrder(Guid workOrderId)
        {
            var result = await mediator.Send(new DeleteWorkOrderCommand(workOrderId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("schedule")]
        [ProducesResponseType(typeof(ScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [EndpointSummary("Retrieves the schedule for a given day.")]
        [EndpointName("GetDailySchedule")]
        [EndpointDescription("Returns a schedule view for the specified date. If no date is provided, today's schedule is returned. You can optionally filter by technician ID.")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSchedule([FromQuery] GetScheduleRequest request)
        {
            var result = await mediator.Send(new GetScheduleQuery(
                request.Day,
                request.TimeZone,
                request.EmployeeId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}

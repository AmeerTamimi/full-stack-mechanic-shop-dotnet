using Azure.Core;
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
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController(IMediator mediator) : ApiController
    {
        [HttpGet]
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
        public async Task<IActionResult> GetWorkOrderById(Guid workOrderId)
        {
            var result = await mediator.Send(new GetWorkOrderByIdQuery(workOrderId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddWorkOrder(CreateWorkOrderRequest request)
        {
            var result = await mediator.Send(new CreateWorkOrderCommand(request.VehicleId,
                                                                        request.CustomerId,
                                                                        request.EmployeeId,
                                                                        request.StartTime,
                                                                        request.Bay,
                                                                        request.WorkOrderRepairTasks
                                                                        .Select(wr => new WorkOrderRepairTasksCommand(wr.RepairTaskId))
                                                                        .ToList()));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPut("technician/{workOrderId:guid}")]
        public async Task<IActionResult> AssignWorkOrderTechnician(Guid workOrderId , AssignTechnicianRequest request)
        {
            var result = await mediator.Send(new AssignTechnicianCommand(workOrderId, request.EmployeeId));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("relocate/{workOrderId:guid}")]
        public async Task<IActionResult> RelocateWorkOrder([FromRoute] Guid workOrderId, RelocateWorkOrderRequest request)
        {
            var result = await mediator.Send(new RelocateWorkOrderCommand(workOrderId , request.StartTime , request.Bay));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("vehicle/{workOrderId:guid}")]
        public async Task<IActionResult> UpdateWorkOrderVehicle(Guid workOrderId, UpdateWorkOrderVehicleRequest request)
        {
            var result = await mediator.Send(new UpdateWorkOrderVehicleCommand(workOrderId, request.VehicleId));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("state/{workOrderId:guid}")]
        public async Task<IActionResult> UpdateWorkOrderState(Guid workOrderId, UpdateWorkOrderStateRequest request)
        {
            var result = await mediator.Send(new UpdateWorkOrderStateCommand(workOrderId,request.State));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpPut("repair-tasks/{workOrderId:guid}")]
        public async Task<IActionResult> UpdateWorkOrderRepairTasks(Guid workOrderId, UpdateWorkOrderRepairTasksRequest request)
        {
            var result = await mediator.Send(new UpdateWorkOrderRepairTasksCommand(workOrderId, request.WorkOrderRepairTasks
                                                                                                 .Select(wr => new WorkOrderRepairTasksCommand(wr.RepairTaskId)).ToList()));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }
        [HttpDelete("{workOrderId:guid}")]
        public async Task<IActionResult> DeleteWorkOrder(Guid workOrderId)
        {
            var result = await mediator.Send(new DeleteWorkOrderCommand(workOrderId));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        //[HttpGet("schedule")]
        //public async Task<IActionResult> GetSchedule(GetScheduleRequest request)
        //{
        //    var result = await mediator.Send(new GetScheduleQuery(request.Day));

        //    return result.Match(
        //        response => Ok(response),
        //        Problem
        //    );
        //}
    }
}

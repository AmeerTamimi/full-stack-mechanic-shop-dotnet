using GOATY.Application.Features.WorkOrders.WorkOrdersCommands;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.CreateWorkOrder;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.DeleteWorkOrder;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.UpdateWorkOrder;
using GOATY.Application.Features.WorkOrders.WorkOrdersCommands.WorkOrderRepairTasksCommands;
using GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrderById;
using GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrders;
using GOATY.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetWorkOrders()
        {
            var result = await mediator.Send(new GetWorkOrdersQuery());

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetWorkOrderById(Guid id)
        {
            var result = await mediator.Send(new GetWorkOrderByIdQuery(id));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddWorkOrder(WorkOrderRequest request)
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateWorkOrder(Guid id , WorkOrderRequest request)
        {
            var result = await mediator.Send(new UpdateWorkOrderCommand(id,
                                                                        request.VehicleId,
                                                                        request.CustomerId,
                                                                        request.EmployeeId,
                                                                        request.StartTime,
                                                                        request.Bay,
                                                                        request.WorkOrderRepairTasks
                                                                        .Select(wr => new WorkOrderRepairTasksCommand(wr.RepairTaskId))
                                                                        .ToList()));

            return result.Match(
                response => NoContent(),
                Problem
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteWorkOrder(Guid id)
        {
            var result = await mediator.Send(new DeleteWorkOrderCommand(id));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}

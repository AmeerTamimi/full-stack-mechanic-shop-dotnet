using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateRepairTaskCommands;
using GOATY.Application.Features.RepairTasks.RepairTaskCommands.DeleteRepairTaskCommands;
using GOATY.Application.Features.RepairTasks.RepairTaskCommands.UpdateRepairTaskCommands;
using GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTaskById;
using GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery;
using GOATY.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairTasksController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetRepairTasks()
        {
            var result = await mediator.Send(new GetRepairTaskQuery());

            return result.Match<IActionResult>(
                    response => Ok(response),
                    Problem
                );
        }

        [HttpGet("{id:guid}")]
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
        public async Task<IActionResult> AddRepairTask(RepairTaskRequest request)
        {
           var result = await mediator.Send(new CreateRepairTaskCommand(request.Name!,
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
        public async Task<IActionResult> UpdateRepairTask(Guid id , RepairTaskRequest request)
        {
            var result = await mediator.Send(new UpdateRepairTaskCommand(id,
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

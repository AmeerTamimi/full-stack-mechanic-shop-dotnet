using GOATY.Application.Features.RepairTasks.RepairTaskCommands.CreateReapairTaskCommands;
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
            return null; 
            //    var result = await mediator.Send(new CreateRepairTaskCommand(request.Name!,
            //                                                                 request.Description!,
            //                                                                 request.TimeEstimated,
            //                                                                 request.CostEstimated,
            //                                                                 request.RepairtTaskDetails));

            //    return result.Match(
            //        response => Ok(response),
            //        Problem
            //    );
        }
    }
}

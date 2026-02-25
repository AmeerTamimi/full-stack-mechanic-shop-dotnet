using GOATY.Application.Features.RepairTasks.RepairTaskQueries.GetRepairTasksQuery;
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

            return result.Match(
                    response => Ok(response),
                    Problem
                );
        }
    }
}

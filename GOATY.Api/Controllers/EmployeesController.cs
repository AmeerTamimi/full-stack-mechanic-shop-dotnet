using GOATY.Application.Queries.EmployeeQueries.GetEmployeesQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var result = await mediator.Send(new GetEmployeesQuery());

            return result.Match(
                response => Ok(response),
                Problem
                );
        }
    }
}

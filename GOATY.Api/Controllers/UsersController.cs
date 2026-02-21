using GOATY.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] EmployeeRequest employee)
        {
            var result = await mediator.Send(new SignUpCommand(employee.FirstName, employee.LastName, employee.Role));

            return result.Match(
                response => Ok(response),
                Problem
                ); 
        }
    }
}

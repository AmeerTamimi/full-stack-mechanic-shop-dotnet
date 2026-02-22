using MediatR;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController(IMediator mediator) : ApiController
    {
        [HttpPost]
        public Task<IActionResult> GenerateToken(string email , string password)
        {
            var result = mediator.Send(new GenerateTokenQuery(email, password));

            return result.Match(
                response => Ok(response),
                Problem
            );
        }
    }
}

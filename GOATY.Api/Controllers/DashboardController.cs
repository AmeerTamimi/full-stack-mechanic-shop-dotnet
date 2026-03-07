using GOATY.Application.Features.Dashboards.DashboardQueries;
using GOATY.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GOATY.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IMediator mediator) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetDashboard(DashboardRequest request)
        {
            var result = await mediator.Send(new GetDashboardQuery(request.Day, TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone)));

            return result.Match(
                    response => Ok(response),
                    Problem
                );
        }
    }
}

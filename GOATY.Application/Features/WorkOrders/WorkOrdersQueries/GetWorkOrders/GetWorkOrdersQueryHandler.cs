using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrders
{
    public sealed class GetWorkOrdersQueryHandler(IAppDbContext context, ILogger<GetWorkOrdersQueryHandler> logger) 
        : IRequestHandler<GetWorkOrdersQuery , Result<List<WorkOrderDto>>>
    {

        public async Task<Result<List<WorkOrderDto>>> Handle(GetWorkOrdersQuery request, CancellationToken cancellationToken)
        {
            var workOrders = await context.WorkOrders
                                         .AsNoTracking()
                                         .Include(wo => wo.WorkOrderRepairTasks)
                                         .ToListAsync();

            return workOrders.ToDtos();
        }
    }
}

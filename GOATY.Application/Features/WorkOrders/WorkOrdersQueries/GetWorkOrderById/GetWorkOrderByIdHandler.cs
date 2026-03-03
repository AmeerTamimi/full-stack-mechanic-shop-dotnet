using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrderById
{
    public sealed class GetWorkOrderByIdQueryHandler(
        IAppDbContext context,
        ILogger<GetWorkOrderByIdQueryHandler> logger)
        : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
    {
        public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken ct)
        {
            var workOrder = await context.WorkOrders
                                         .Include(w => w.WorkOrderRepairTasks)
                                         .AsNoTracking()
                                         .SingleOrDefaultAsync(w => w.Id == request.Id, ct);

            if (workOrder is null)
            {
                return Error.NotFound(
                    code: "WorkOrder_NotFound",
                    description: $"WorkOrder With Id {request.Id} was Not Found"
                );
            }

            return workOrder.ToDto();
        }
    }
}
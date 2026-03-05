using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Dashboards.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Dashboards.DashboardQueries
{
    public sealed class GetDashboardQueryHandler(IAppDbContext context)
        : IRequestHandler<GetDashboardQuery, Result<Dashboard>>
    {
        public async Task<Result<Dashboard>> Handle(GetDashboardQuery request, CancellationToken ct)
        {
            var workOrders = await context.WorkOrders
                                           .AsNoTracking()
                                           .Include(wo => wo.Customer)
                                           .Include(wo => wo.Vehicle)
                                           .Include(wo => wo.Employee)
                                           .Include(wo => wo.WorkOrderRepairTasks)
                                                .ThenInclude(wr => wr.RepairTask)
                                                    .ThenInclude(r => r.RepairTaskDetails)
                                                        .ThenInclude(rd => rd.Part)
                                           .ToListAsync(ct);



            var totalOrders = CountOrders(workOrders);

            //var totalScheduled
            return null!;
        }
        private int CountOrders(List<WorkOrder> workOrders)
        {
            return workOrders.Count;
        }
        private int CountState(List<WorkOrder> workOrders , State state)
        {
            return workOrders.Where(wo => wo.State == state).Count();
        }
        private int CountRevenue(List<WorkOrder> workOrders)
        {
            return -1;
        }
        private decimal CountPartsCost(List<WorkOrder> workOrders)
        {
            return workOrders.Sum(wo => wo.WorkOrderRepairTasks
                             .Sum(wr => wr.RepairTask.RepairTaskDetails
                             .Sum(rd => rd.UnitPrice)));
                                   
        }
        private int CountVehicles(List<WorkOrder> workOrders)
        {
            return workOrders.Select(wo => wo.VehicleId).Distinct().Count();
        }
        private int CountCustomer(List<WorkOrder> workOrders)
        {
            return workOrders.Select(wo => wo.CustomerId).Distinct().Count();
        }
        private double CountStateRate(List<WorkOrder> workOrders , State state)
        {
            return (double)CountState(workOrders, state) / CountOrders(workOrders);
        }
        //private double CountOrdersPerVehicle(List<WorkOrder> workOrders, State state)
        //{
        //    return 
        //}

    }
}

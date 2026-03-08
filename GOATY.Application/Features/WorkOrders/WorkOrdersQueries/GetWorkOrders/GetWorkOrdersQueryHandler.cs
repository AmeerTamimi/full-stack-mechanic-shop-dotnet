using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Customers.Mappers;
using GOATY.Application.Features.Employees.Mapping;
using GOATY.Application.Features.WorkOrders.DTOs;
using GOATY.Application.Features.WorkOrders.Mappers;
using GOATY.Domain.Common.Results;
using GOATY.Domain.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.WorkOrders.WorkOrdersQueries.GetWorkOrders
{
    public sealed class GetWorkOrdersQueryHandler(IAppDbContext context, ILogger<GetWorkOrdersQueryHandler> logger) 
        : IRequestHandler<GetWorkOrdersQuery , Result<PaginatedList<WorkOrderDto>>>
    {

        public async Task<Result<PaginatedList<WorkOrderDto>>> Handle(GetWorkOrdersQuery query, CancellationToken ct)
        {
            var workOrdersQuery = context.WorkOrders
                .AsNoTracking()
                .Include(wo => wo.Customer)
                    .ThenInclude(c => c.Vehicles)
                .Include(wo => wo.Vehicle)
                .Include(wo => wo.Employee)
                .Include(wo => wo.WorkOrderRepairTasks)
                    .ThenInclude(wr => wr.RepairTask)
                        .ThenInclude(r => r.RepairTaskDetails)
                            .ThenInclude(r => r.Part)
                .AsQueryable();

            workOrdersQuery = ApplyFilters(workOrdersQuery, query);

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                workOrdersQuery = ApplySearchTerm(workOrdersQuery, query.SearchTerm);
            }

            workOrdersQuery = ApplySorting(workOrdersQuery, query.SortColumn, query.SortDirection);

            var count = await workOrdersQuery.CountAsync(cancellationToken: ct);

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 10, 100);

            var items = await workOrdersQuery
                  .Skip((page - 1) * pageSize)
                  .Take(pageSize)
                  .Select(wo => new WorkOrderDto
                  {
                      Id = wo.Id,
                      Bay = wo.Bay,
                      StartTime = wo.StartTime,
                      EndTime = wo.EndTime,
                      Vehicle = wo.Vehicle!.ToDto(),
                      Customer = wo.Vehicle!.Customer!.ToDto(),
                      Employee = wo.Employee != null? wo.Employee.ToDto() : null,
                      State = wo.State,
                      RepairTasks = wo.WorkOrderRepairTasks.ToDtos()
                  })
                .ToListAsync(ct);

            return new PaginatedList<WorkOrderDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }

        private static IQueryable<WorkOrder> ApplyFilters(IQueryable<WorkOrder> query, GetWorkOrdersQuery searchQuery)
        {
            if (searchQuery.State.HasValue)
            {
                query = query.Where(wo => wo.State == searchQuery.State.Value);
            }

            if (searchQuery.VehicleId.HasValue && searchQuery.VehicleId != Guid.Empty)
            {
                query = query.Where(wo => wo.VehicleId == searchQuery.VehicleId.Value);
            }

            if (searchQuery.LaborId.HasValue && searchQuery.LaborId != Guid.Empty)
            {
                query = query.Where(wo => wo.EmployeeId == searchQuery.LaborId.Value);
            }

            if (searchQuery.StartDateFrom.HasValue)
            {
                query = query.Where(wo => wo.StartTime >= searchQuery.StartDateFrom.Value);
            }

            if (searchQuery.StartDateTo.HasValue)
            {
                query = query.Where(wo => wo.StartTime <= searchQuery.StartDateTo.Value);
            }

            if (searchQuery.EndDateFrom.HasValue)
            {
                query = query.Where(wo => wo.EndTime >= searchQuery.EndDateFrom.Value);
            }

            if (searchQuery.EndDateTo.HasValue)
            {
                query = query.Where(wo => wo.EndTime <= searchQuery.EndDateTo.Value);
            }

            if (searchQuery.Bay.HasValue)
            {
                query = query.Where(wo => wo.Bay == searchQuery.Bay.Value);
            }

            return query;
        }

        private static IQueryable<WorkOrder> ApplySearchTerm(IQueryable<WorkOrder> query, string searchTerm)
        {
            var normalized = searchTerm.Trim().ToLower();

            return query.Where(wo =>
                (wo.Vehicle != null && (
                    wo.Vehicle.Brand.ToLower().Contains(normalized) ||
                    wo.Vehicle.Model.ToLower().Contains(normalized) ||
                    wo.Vehicle.LicensePlate.ToLower().Contains(normalized)
                )) ||
                (wo.Employee != null && (
                    wo.Employee.FirstName.ToLower().Contains(normalized) ||
                    wo.Employee.LastName.ToLower().Contains(normalized) ||
                    (wo.Employee.FirstName + " " + wo.Employee.LastName).ToLower().Contains(normalized)
                )) ||
                wo.WorkOrderRepairTasks.Any(rt =>
                    rt.RepairTask.Name!.ToLower().Contains(normalized)) ||
                wo.Id.ToString().ToLower().Contains(normalized));
        }

        private static IQueryable<WorkOrder> ApplySorting(IQueryable<WorkOrder> query, string sortColumn, string sortDirection)
        {
            var isDescending = sortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase);

            return sortColumn.ToLower() switch
            {
                "createdat" => isDescending ? query.OrderByDescending(wo => wo.CreatedAtUtc) : query.OrderBy(wo => wo.CreatedAtUtc),
                "updatedat" => isDescending ? query.OrderByDescending(wo => wo.LastModifiedUtc) : query.OrderBy(wo => wo.LastModifiedUtc),
                "startat" => isDescending ? query.OrderByDescending(wo => wo.StartTime) : query.OrderBy(wo => wo.StartTime),
                "endat" => isDescending ? query.OrderByDescending(wo => wo.EndTime) : query.OrderBy(wo => wo.EndTime),
                "state" => isDescending ? query.OrderByDescending(wo => wo.State) : query.OrderBy(wo => wo.State),
                "spot" => isDescending ? query.OrderByDescending(wo => wo.Bay) : query.OrderBy(wo => wo.Bay),
                "totaltime" => isDescending ? query.OrderByDescending(wo => wo.TotalTime) : query.OrderBy(wo => wo.TotalTime),
                "totalcost" => isDescending ? query.OrderByDescending(wo => wo.TotalCost) : query.OrderBy(wo => wo.TotalCost),
                "vehicleid" => isDescending ? query.OrderByDescending(wo => wo.VehicleId) : query.OrderBy(wo => wo.VehicleId),
                "laborid" => isDescending ? query.OrderByDescending(wo => wo.EmployeeId) : query.OrderBy(wo => wo.EmployeeId),
                _ => query.OrderByDescending(wo => wo.CreatedAtUtc) // Default sorting
            };
        }
    }
}

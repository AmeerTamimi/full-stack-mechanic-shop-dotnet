using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Application.Features.Customers.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Customers.CustomerQueries.GetCustomers
{
    public sealed class GetCustomersQueryHandler(
        IAppDbContext context,
        ILogger<GetCustomersQueryHandler> logger)
        : IRequestHandler<GetCustomersQuery, Result<PaginatedList<CustomerDto>>>
    {
        public async Task<Result<PaginatedList<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken ct)
        {
            var customersQuery = context.Customers
                                .Include(c => c.Vehicles)
                                .AsNoTracking()
                                .AsQueryable();

            var count = await customersQuery.CountAsync(ct);

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var customers = await customersQuery
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync(ct);

            return new PaginatedList<CustomerDto>
            {
                Items = customers.ToDtos(),
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize)
            };
        }
    }
}
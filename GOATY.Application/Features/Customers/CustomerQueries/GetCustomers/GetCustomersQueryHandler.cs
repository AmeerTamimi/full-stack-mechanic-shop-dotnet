using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Application.Features.Customers.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Customers.CustomerQueries.GetCustomers
{
    public sealed class GetCustomersQueryHandler(
        IAppDbContext context,
        ILogger<GetCustomersQueryHandler> logger)
        : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
    {
        public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken ct)
        {
            var customers = await context.Customers
                                .Include(c => c.Vehicles)
                                .AsNoTracking()
                                .ToListAsync(ct);

            return customers.ToDtos();
        }
    }
}

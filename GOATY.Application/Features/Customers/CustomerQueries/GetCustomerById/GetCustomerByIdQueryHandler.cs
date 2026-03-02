using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Application.Features.Customers.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Customers.CustomerQueries.GetCustomerById
{
    public sealed class GetCustomerByIdQueryHandler(IAppDbContext context, ILogger<GetCustomerByIdQueryHandler> logger) : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
    {
        public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken ct)
        {
            var customer = await context.Customers
                                        .Include(c => c.Vehicles)
                                        .SingleOrDefaultAsync(c => c.Id == request.Id , ct);

            if(customer is null)
            {
                return Error.NotFound(
                    code: "Customer_NotFound",
                    description: $"Customer With Id {request.Id} was Not Found"
                );
            }

            return customer.ToDto();
        }
    }
}

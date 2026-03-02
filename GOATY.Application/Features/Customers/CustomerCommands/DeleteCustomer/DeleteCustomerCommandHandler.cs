using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Application.Features.Customers.Mappers;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Customers.CustomerCommands.DeleteCustomer
{
    public sealed class DeleteCustomerCommandHandler(IAppDbContext context, ILogger<DeleteCustomerCommandHandler> logger, HybridCache cache) : IRequestHandler<DeleteCustomerCommand, Result<CustomerDto>>
    {
        public async Task<Result<CustomerDto>> Handle(DeleteCustomerCommand request, CancellationToken ct)
        {
            var customer = await context.Customers
                                        .Include(c => c.Vehicles)
                                        .SingleOrDefaultAsync(c => c.Id == request.Id, ct);

            if (customer is null)
            {
                return Error.NotFound(
                    code: "Customer_NotFound",
                    description: $"Customer With Id {request.Id} was Not Found"
                );
            }

            context.Customers.Remove(customer);
            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("customers");

            return customer.ToDto();
        }
    }
}

using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Customers.CustomerQueries.GetCustomers
{
    public sealed class GetCustomersQuery : ICachedQuery<Result<List<CustomerDto>>>
    {
        public string CacheKey => "customers";

        public string[] Tags => ["customers"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}

using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Customers.CustomerQueries.GetCustomerById
{
    public sealed record class GetCustomerByIdQuery(Guid Id) : ICachedQuery<Result<CustomerDto>>
    {
        public string CacheKey => $"customers_{Id}";

        public string[] Tags => ["customers"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}

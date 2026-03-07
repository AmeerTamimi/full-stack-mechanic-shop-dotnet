using GOATY.Application.Common.Interfaces;
using GOATY.Application.Common.Models;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Customers.CustomerQueries.GetCustomers
{
    public sealed record class GetCustomersQuery(int Page, int PageSize)
        : ICachedQuery<Result<PaginatedList<CustomerDto>>>
    {
        public string CacheKey =>
            $"customers:" +
            $"p={Page}:" +
            $"ps={PageSize}";

        public string[] Tags => ["customers"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
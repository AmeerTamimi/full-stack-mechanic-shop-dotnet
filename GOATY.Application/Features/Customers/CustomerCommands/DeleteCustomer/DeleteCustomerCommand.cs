using GOATY.Application.Features.Customers.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Customers.CustomerCommands.DeleteCustomer
{
    public sealed record class DeleteCustomerCommand(Guid Id) : IRequest<Result<CustomerDto>>;
}

using GOATY.Application.Features.Customers.CustomerCommands.UpdateCustomer;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Customers.CustomerCommands.CreateCustomer
{
    public sealed record class CreateCustomerCommand(
        string FirstName,
        string LastName,
        string Phone,
        string Email,
        string Address,
        IReadOnlyList<CreateVehicleCommand> Vehicles) 
        : IRequest<Result<CustomerDto>>;
}

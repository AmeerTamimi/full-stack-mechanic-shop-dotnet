using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Customers.CustomerCommands.UpdateCustomer
{
    public sealed record class UpdateCustomerCommand(
        Guid Id,
        string FirstName,
        string LastName,
        string Phone,
        string Email,
        string Address,
        IReadOnlyList<VehicleRequirements> Vehicles) 
        : IRequest<Result<Updated>>;
}

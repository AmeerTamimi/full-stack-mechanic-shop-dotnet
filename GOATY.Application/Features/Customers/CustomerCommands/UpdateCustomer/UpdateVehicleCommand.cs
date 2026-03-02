namespace GOATY.Application.Features.Customers.CustomerCommands.UpdateCustomer
{
    public sealed record class UpdateVehicleCommand(Guid Id,
                                                   string Brand,
                                                   string Model,
                                                   int Year,
                                                   string LicensePlate);

}

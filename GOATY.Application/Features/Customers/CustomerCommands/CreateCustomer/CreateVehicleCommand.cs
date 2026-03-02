namespace GOATY.Application.Features.Customers.CustomerCommands.CreateCustomer
{
    public sealed record class CreateVehicleCommand(string Brand,
                                                    string Model,
                                                    int Year,
                                                    string LicensePlate);

}

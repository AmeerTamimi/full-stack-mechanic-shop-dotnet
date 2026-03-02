namespace GOATY.Application.Features.Customers.CustomerCommands
{
    public sealed class VehicleRequirements
    {
        public Guid CustomerId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
    }
}

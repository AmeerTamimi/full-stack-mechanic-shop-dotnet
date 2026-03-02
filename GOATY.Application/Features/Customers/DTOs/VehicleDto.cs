using GOATY.Domain.Customers;

namespace GOATY.Application.Features.Customers.DTOs
{
    public sealed class VehicleDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public string VehicleInfo => $"{Brand} | {Model} | {Year}";
    }
}

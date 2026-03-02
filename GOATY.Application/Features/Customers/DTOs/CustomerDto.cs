using GOATY.Domain.Customers.Vehicles;

namespace GOATY.Application.Features.Customers.DTOs
{
    public sealed class CustomerDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public IEnumerable<VehicleDto> Vehicles { get; set; } = [];
    }
}

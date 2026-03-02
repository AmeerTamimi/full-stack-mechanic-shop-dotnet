using GOATY.Application.Features.Customers.DTOs;
using GOATY.Domain.Customers.Vehicles;

namespace GOATY.Application.Features.Customers.Mappers
{
    public static class VehicleMapper
    {
        public static VehicleDto ToDto(this Vehicle model)
        {
            return new VehicleDto
            {
                Id = model.Id,
                CustomerId = model.CustomerId,
                Brand = model.Brand,
                Model = model.Model,
                Year = model.Year,
                LicensePlate = model.LicensePlate

            };
        }
        public static IEnumerable<VehicleDto> ToDtos(this IEnumerable<Vehicle> models)
        {
            return models.Select(v => v.ToDto());
        }
    }
}

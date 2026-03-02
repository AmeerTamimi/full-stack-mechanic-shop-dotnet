using GOATY.Application.Features.Customers.DTOs;
using GOATY.Domain.Customers;

namespace GOATY.Application.Features.Customers.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer model)
        {
            return new CustomerDto
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Vehicles = model.Vehicles.ToDtos()
            };
        }

        public static List<CustomerDto> ToDtos(this List<Customer> models)
        {
            return models.ConvertAll(c => c.ToDto());
        }
    }
}

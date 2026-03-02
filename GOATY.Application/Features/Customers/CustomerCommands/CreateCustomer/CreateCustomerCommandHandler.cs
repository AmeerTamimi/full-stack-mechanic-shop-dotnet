using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Customers.DTOs;
using GOATY.Application.Features.Customers.Mappers;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Customers.CustomerCommands.CreateCustomer
{
    public sealed class CreateCustomerCommandHandler(
        IAppDbContext context,
        ILogger<CreateCustomerCommandHandler> logger,
        HybridCache cache) 
        : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken ct)
        {
            var customerId = Guid.NewGuid();



            var plates = request.Vehicles.Select(v => v.LicensePlate.Trim().ToUpper());

            var duplicatedPlateInRequest = plates.GroupBy(p => p)
                                           .Where(g => g.Count() > 1)
                                           .Select(g => g.Key)
                                           .ToList();

            if(duplicatedPlateInRequest.Count > 0)
            {
                var plate = duplicatedPlateInRequest[0];
                return Error.Conflict(
                    code: "Vehicle.Plate.DuplicateInRequest",
                    description: $"Plate {plate} is duplicated in the request."
                );
            }

            var duplicatedPlateInDb = await context.Vehicles
                                                   .AsNoTracking()
                                                   .Where(v => plates.Contains(v.LicensePlate.Trim().ToUpper()))
                                                   .Select(v => v.LicensePlate)
                                                   .ToListAsync(ct);

            if (duplicatedPlateInDb.Count > 0)
            {
                var plate = duplicatedPlateInDb[0];
                return Error.Conflict(
                    code: "Vehicle.Plate.Conflict",
                    description: $"A Vehicle With Plate {plate} Already Exists."
                );
            }

            var vehicles = new List<Vehicle>();

            foreach (var vehicle in request.Vehicles)
            {
                var vehicleModel = Vehicle.Create(customerId,
                                                  vehicle.Brand,
                                                  vehicle.Model,
                                                  vehicle.Year,
                                                  vehicle.LicensePlate);

                if (!vehicleModel.IsSuccess)
                {
                    return vehicleModel.Errors;
                }

                vehicles.Add(vehicleModel.Value);
            }

            var newCustomer = Customer.Create(customerId,
                                              request.FirstName,
                                              request.LastName,
                                              request.Phone,
                                              request.Email,
                                              request.Address,
                                              vehicles);


            if (!newCustomer.IsSuccess)
            {
                return newCustomer.Errors;
            }

            await context.Customers.AddAsync(newCustomer.Value , ct);
            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("customers" , ct);

            return newCustomer.Value.ToDto();
        }
    }
}

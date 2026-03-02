using GOATY.Application.Common.Interfaces;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace GOATY.Application.Features.Customers.CustomerCommands.UpdateCustomer
{
    public sealed class UpdateCustomerCommandHandler(IAppDbContext context, ILogger<UpdateCustomerCommandHandler> logger, HybridCache cache) : IRequestHandler<UpdateCustomerCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateCustomerCommand request, CancellationToken ct)
        {
            var customer = await context.Customers
                                        .Include(c => c.Vehicles)
                                        .SingleOrDefaultAsync(c => c.Id == request.Id , ct);

            if (customer is null)
            {
                return Error.NotFound(
                    code: "Customer_NotFound",
                    description: $"Customer With Id {request.Id} was Not Found"
                );
            }

            var plates = request.Vehicles.Select(v => v.LicensePlate.Trim().ToUpper());

            var duplicatedPlatesInRequest = plates.GroupBy(p => p)
                                                  .Where(g => g.Count() > 1)
                                                  .Select(g => g.Key)
                                                  .ToList();

            if(duplicatedPlatesInRequest.Count > 0)
            {
                var plate = duplicatedPlatesInRequest[0];
                return Error.Conflict(
                    code: "Vehicle.Plate.DuplicateInRequest",
                    description: $"Plate {plate} is duplicated in the request."
                );
            }

            var duplicatedPlatesInDb = await context.Vehicles
                                                    .Where(v => plates.Contains(v.LicensePlate.Trim().ToUpper()))
                                                    .Where(v => v.CustomerId != customer.Id)
                                                    .Select(v => v.LicensePlate)
                                                    .ToListAsync(ct);

            if (duplicatedPlatesInDb.Count > 0)
            {
                var plate = duplicatedPlatesInDb[0];
                return Error.Conflict(
                    code: "Vehicle.Plate.Conflict",
                    description: $"A Vehicle With Plate {plate} Already Exists."
                );
            }

            var vehicles = new List<Vehicle>();

            foreach (var vehicle in request.Vehicles)
            {
                var vehicleModel = Vehicle.Create(customer.Id,
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

            var updateResult = customer.Update(request.FirstName,
                                               request.LastName,
                                               request.Phone,
                                               request.Email,
                                               request.Address);

            if (!updateResult.IsSuccess)
            {
                return updateResult.Errors;
            }

            var upsertResult = customer.UpsertVehicles(vehicles);

            if (!upsertResult.IsSuccess)
            {
                return upsertResult.Errors;
            }

            await context.SaveChangesAsync(ct);

            await cache.RemoveByTagAsync("customers" , ct);

            return Result.Updated;
        }
    }
}

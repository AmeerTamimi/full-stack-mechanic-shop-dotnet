using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Customers.Vehicles
{
    public sealed class Vehicle : AuditableEntity
    {
        public Guid CustomerId { get; private set; }
        public string Brand { get; private set; }
        public string Model { get; private set; }
        public int Year { get; private set; }
        public string LicensePlate { get; private set; }
        public Customer? Customer { get; set; }
        public string VehicleInfo => $"{Brand} | {Model} | {Year}";

        private Vehicle() { }
        private Vehicle(Guid id,Guid customerId, string brand, string model, int year, string licensePlate)
        : base(id)
        {
            CustomerId = customerId;
            Brand = brand;
            Model = model;
            Year = year;
            LicensePlate = licensePlate;
        }

        public static Result<Vehicle> Create(Guid customerId,
                                             string brand,
                                             string model,
                                             int year,
                                             string licensePlate)
        {

            if (Guid.Empty == customerId)
            {
                return VehicleErrors.CustomerIdRequired;
            }
            if (string.IsNullOrWhiteSpace(brand))
            {
                return VehicleErrors.BrandRequired;
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                return VehicleErrors.ModelRequired;
            }

            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                return VehicleErrors.LicensePlateRequired;
            }

            if (year < 1886 || year > DateTime.UtcNow.Year)
            {
                return VehicleErrors.YearInvalid;
            }

            var id = Guid.NewGuid();

            return new Vehicle(id,customerId, brand, model, year, licensePlate);
        }

        public Result<Updated> Update(Guid customerId,
                                      string brand,
                                      string model,
                                      int year,
                                      string licensePlate)
        {

            if(Guid.Empty == customerId)
            {
                return VehicleErrors.CustomerIdRequired;
            }
            if (string.IsNullOrWhiteSpace(brand))
            {
                return VehicleErrors.BrandRequired;
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                return VehicleErrors.ModelRequired;
            }

            if (year < 1886 || year > DateTime.UtcNow.Year)
            {
                return VehicleErrors.YearInvalid;
            }

            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                return VehicleErrors.LicensePlateRequired;
            }

            CustomerId = customerId;
            Brand = brand;
            Model = model;
            Year = year;
            LicensePlate = licensePlate;

            return Result.Updated;
        }
        public override string ToString()
        {
            return $"Vehicle(Id={Id}, CustomerId={CustomerId}, Plate='{LicensePlate}', Brand='{Brand}', Model='{Model}', Year={Year})";
        }
    }
}

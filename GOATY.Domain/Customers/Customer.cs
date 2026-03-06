using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Customers.Vehicles;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GOATY.Domain.Customers
{
    public sealed class Customer : AuditableEntity
    {
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? FullName { get; private set; }
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? Address { get; private set; }

        private readonly List<Vehicle> _vehicles = [];
        public IReadOnlyCollection<Vehicle> Vehicles  => _vehicles.AsReadOnly();

        private readonly List<WorkOrder> _workOrders = [];
        public IReadOnlyCollection<WorkOrder> WorkOrders => _workOrders.AsReadOnly();

        private Customer() { }
        private Customer(Guid id,
                         string firstName,
                         string lastName,
                         string phone,
                         string email,
                         string address,
                         List<Vehicle> vehicles) : base(id) 
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            FullName = $"{firstName} {lastName}";
            Phone = phone;
            Email = email;
            Address = address;
            _vehicles = vehicles;
        }


        public static Result<Customer> Create(Guid id,
                                       string firstName,
                                       string lastName,
                                       string phone,
                                       string email,
                                       string address,
                                       List<Vehicle> vehicles)
        {
            if(string.IsNullOrWhiteSpace(firstName) ||
                                         firstName.Length > 50 ||
                                         firstName.Length < 5)
            {
                return CustomerErrors.InvalidFirstName;
            }
            if (string.IsNullOrWhiteSpace(lastName) ||
                                         lastName.Length > 50 ||
                                         lastName.Length < 5)
            {
                return CustomerErrors.InvalidLastName;
            }

            if (string.IsNullOrWhiteSpace(lastName) ||
                                         lastName.Length > 50 ||
                                         lastName.Length < 5)
            {
                return CustomerErrors.InvalidLastName;
            }
            if(!Regex.IsMatch(phone , @"^(?:0|970|972)5(6|9)\d{7}$"))
            {
                return CustomerErrors.InvalidPhoneNumber;
            }
            if (!MailAddress.TryCreate(email, out _))
            {
                return CustomerErrors.InvalidEmail;
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                return CustomerErrors.InvalidAddress;
            }
            if (vehicles is null)
            {
                return CustomerErrors.InvalidVehicles;
            }

            return new Customer(id, firstName, lastName, phone, email, address, vehicles);
        }

        public Result<Updated> Update(string firstName,
                                       string lastName,
                                       string phone,
                                       string email,
                                       string address)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                                         firstName.Length > 50 ||
                                         firstName.Length < 5)
            {
                return CustomerErrors.InvalidFirstName;
            }
            if (string.IsNullOrWhiteSpace(lastName) ||
                                         lastName.Length > 50 ||
                                         lastName.Length < 5)
            {
                return CustomerErrors.InvalidLastName;
            }

            if (string.IsNullOrWhiteSpace(lastName) ||
                                         lastName.Length > 50 ||
                                         lastName.Length < 5)
            {
                return CustomerErrors.InvalidLastName;
            }
            if (!Regex.IsMatch(phone, @"^(?:0|970|972)5(6|9)\d{7}$"))
            {
                return CustomerErrors.InvalidPhoneNumber;
            }
            if (!MailAddress.TryCreate(email, out _))
            {
                return CustomerErrors.InvalidEmail;
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                return CustomerErrors.InvalidAddress;
            }
            
            FirstName = firstName;
            LastName = lastName;
            FullName = $"{firstName} {lastName}";
            Phone = phone;
            Email = email;
            Address = address;
            
            return Result.Updated;
        }

        public Result<Updated> UpsertVehicles(List<Vehicle> incoming)
        {
            if (incoming is null || incoming.Count() <= 0)
            {
                return CustomerErrors.InvalidVehicles;
            }

            _vehicles.RemoveAll(existing => !incoming.Any(v => v.Id == existing.Id));

            foreach(var incomingVehicle in incoming)
            {
                var existing = _vehicles.SingleOrDefault(v => v.Id == incomingVehicle.Id);

                if(existing is null)
                {
                    var addResult = Vehicle.Create(Id,
                                                   incomingVehicle.Brand,
                                                   incomingVehicle.Model,
                                                   incomingVehicle.Year,
                                                   incomingVehicle.LicensePlate);

                    if (!addResult.IsSuccess)
                    {
                        return addResult.Errors;
                    }

                    _vehicles.Add(addResult.Value);
                }
                else
                {
                    var updateResult = existing.Update(Id,
                                                       incomingVehicle.Brand,
                                                       incomingVehicle.Model,
                                                       incomingVehicle.Year,
                                                       incomingVehicle.LicensePlate);

                    if (!updateResult.IsSuccess)
                    {
                        return updateResult.Errors;
                    }
                }
            }

            return Result.Updated;
        }
    }
}

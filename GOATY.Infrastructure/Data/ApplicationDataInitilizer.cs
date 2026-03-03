using GOATY.Domain.Common.Enums;
using GOATY.Domain.Customers;
using GOATY.Domain.Customers.Vehicles;
using GOATY.Domain.Employees;
using GOATY.Domain.Employees.Enums;
using GOATY.Domain.Parts;
using GOATY.Domain.RepairTasks;
using GOATY.Domain.WorkOrders;
using GOATY.Domain.WorkOrders.Enums;
using GOATY.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GOATY.Infrastructure.Data
{
    public sealed class ApplicationDataInitilizer
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly ILogger<ApplicationDataInitilizer> _logger;
        public ApplicationDataInitilizer(UserManager<AppUser> userManager,
                                  RoleManager<IdentityRole> roleManager,
                                  AppDbContext context,
                                  ILogger<ApplicationDataInitilizer> logger)

        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        public async Task InitilizeAsync()
        {
            try
            {
                await _context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occured while initilizing the database");
                throw;

            }
        }

        public async Task TryToSeedAsync()
        {
            var managerRole = new IdentityRole(nameof(Role.Manager));

            if(!await _roleManager.Roles.AnyAsync(r => r.Name == managerRole.Name)) // runs first time only
            {
                await _roleManager.CreateAsync(managerRole);
            }

            var technicianRole = new IdentityRole(nameof(Role.Technician));

            if(!await _roleManager.Roles.AnyAsync(r => r.Name == technicianRole.Name))
            {
                await _roleManager.CreateAsync(technicianRole);
            }


            var manager = new AppUser
            {
                Id = "12345678-1234-1234-1234-12345678",
                UserName = "AmeerTamimi",
                Email = "a@gmail.com",
                EmailConfirmed = true
            };

            if(!await _userManager.Users.AnyAsync(u => u.Email == manager.Email))
            {
                var createdRes = await _userManager.CreateAsync(manager , manager.Email); // email as a password :)
                
                if (!createdRes.Succeeded)
                {
                    _logger.LogError("Failed to create user {Email}. Errors: {Errors}",
                        manager.Email,
                        createdRes.Errors);
                }

                var roleRes = await _userManager.AddToRoleAsync(manager, managerRole.Name!);

                if (!roleRes.Succeeded)
                {
                    _logger.LogError("Failed To Attach Role to user {Email}. Error: {Errors}",
                        manager.Email,
                        roleRes.Errors);
                }

            }

            var tech1 = new AppUser
            {
                Id = "12345678-1234-1234-1234-11111111",
                UserName = "ImaTechnician",
                Email = "t1@gmail.com",
                EmailConfirmed = true
            };

            if(!await _userManager.Users.AnyAsync(u => u.Email == tech1.Email))
            {
                var createdRes = await _userManager.CreateAsync(tech1, tech1.Email);

                if (!createdRes.Succeeded)
                {
                    _logger.LogError("Failed to create user {Email}. Errors: {Errors}",
                        manager.Email,
                        createdRes.Errors);
                }

                var roleRes = await _userManager.AddToRoleAsync(tech1, technicianRole.Name!);

                if (!roleRes.Succeeded)
                {
                    _logger.LogError("Failed To Attach Role to user {Email}. Error: {Errors}",
                        manager.Email,
                        roleRes.Errors);
                }
            }

            if (!await _context.Parts.AnyAsync())
            {
                _context.Parts.AddRange(
                [
                    Part.Create(
                        id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        name: "Engine Oil",
                        cost: 25.50m,
                        quantity: 100
                    ).Value,

                    Part.Create(
                        id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        name: "Brake Pads",
                        cost: 45.00m,
                        quantity: 50
                    ).Value,

                    Part.Create(
                        id: Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        name: "Air Filter",
                        cost: 15.75m,
                        quantity: 200
                    ).Value,

                    Part.Create(
                        id: Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        name: "Spark Plug",
                        cost: 8.99m,
                        quantity: 300
                    ).Value,

                    Part.Create(
                        id: Guid.Parse("55555555-5555-5555-5555-555555555555"),
                        name: "Battery",
                        cost: 120.00m,
                        quantity: 20
                    ).Value,
                ]);
            }

            if (!await _context.Employees.AnyAsync())
            {
                _context.Employees.AddRange(
                [
                    Employee.Create(
                        id: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                        firstName: "Ameer",
                        lastName: "Tamimi",
                        role: Role.Manager
                    ).Value,

                    Employee.Create(
                        id: Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                        firstName: "Sara",
                        lastName: "Nasser",
                        role: Role.Technician
                    ).Value,

                    Employee.Create(
                        id: Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                        firstName: "Omar",
                        lastName: "Saleh",
                        role: Role.Technician
                    ).Value,

                    Employee.Create(
                        id: Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                        firstName: "Lina",
                        lastName: "Khalil",
                        role: Role.Technician
                    ).Value,

                    Employee.Create(
                        id: Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                        firstName: "Yousef",
                        lastName: "Amir",
                        role: Role.Technician
                    ).Value,
                ]);
            }

            if(! await _context.RepairTasks.AnyAsync())
            {
                var partId = (await _context.Parts.FirstAsync()).Id;

                await _context.AddRangeAsync([
                        RepairTask.Create(
                            id: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                            name: "Battery Repair",
                            desc: "We will ruin the battery lowkey :)",
                            time : TimeStamps.Min45,
                            cost: 300,
                            repairTaskDetails: new List<RepairTaskDetails>{
                                RepairTaskDetails.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),partId ,2 , 30).Value
                            }
                        ).Value
                    ]);
            }

            if (!await _context.Customers.AnyAsync())
            {
                var c1Id = Guid.Parse("f1111111-1111-1111-1111-111111111111");
                var c2Id = Guid.Parse("f2222222-2222-2222-2222-222222222222");

                var customer1 = Customer.Create(
                    id: c1Id,
                    firstName: "Mohammad",
                    lastName: "Hassan",
                    phone: "0591234567",
                    email: "mohammad.hassan@gmail.com",
                    address: "Nablus - Rafidia",
                    vehicles: new List<Vehicle>
                    {
                        Vehicle.Create(c1Id, "Toyota", "Corolla", 2018, "NAP-12345").Value,
                        Vehicle.Create(c1Id, "Hyundai", "Elantra", 2020, "NAP-67890").Value,
                    }
                ).Value;

                var customer2 = Customer.Create(
                    id: c2Id,
                    firstName: "Khaled",
                    lastName: "Yousef",
                    phone: "0569876543",
                    email: "khaled.yousef@gmail.com",
                    address: "Ramallah - Al-Masyoun",
                    vehicles: new List<Vehicle>
                    {
                        Vehicle.Create(c2Id, "Kia", "Sportage", 2019, "RAM-24680").Value,
                        Vehicle.Create(c2Id, "BMW", "X3", 2021, "RAM-13579").Value,
                    }
                ).Value;

                _context.Customers.AddRange([customer1, customer2]);
            }

            if (!await _context.WorkOrders.AnyAsync())
            {
                var batteryRepairTaskId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

                var batteryRepairTask =
                    _context.RepairTasks.Local.FirstOrDefault(r => r.Id == batteryRepairTaskId)
                    ?? await _context.RepairTasks.SingleAsync(r => r.Id == batteryRepairTaskId);

                var vehicle1 =
                    _context.Vehicles.Local.FirstOrDefault(v => v.LicensePlate == "NAP-12345")
                    ?? await _context.Vehicles.SingleAsync(v => v.LicensePlate == "NAP-12345");

                var vehicle2 =
                    _context.Vehicles.Local.FirstOrDefault(v => v.LicensePlate == "RAM-13579")
                    ?? await _context.Vehicles.SingleAsync(v => v.LicensePlate == "RAM-13579");

                var tech1Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
                var tech2Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

                var wo1Id = Guid.Parse("99999999-9999-9999-9999-999999999991");

                var wo1RepairTasks = new List<WorkOrderRepairTasks>
                    {
                        WorkOrderRepairTasks.Create(
                            workOrderId: wo1Id,
                            repairTaskId: batteryRepairTask.Id,
                            time: batteryRepairTask.TimeEstimated,
                            cost: batteryRepairTask.CostEstimated
                        ).Value
                    };

                var workOrder1 = WorkOrder.Create(
                    id: wo1Id,
                    vehicleId: vehicle1.Id,
                    customerId: vehicle1.CustomerId,
                    employeeId: tech1Id,
                    startTime: DateTime.Now.AddDays(1),
                    bay: Bay.A,
                    repairTasks: wo1RepairTasks
                ).Value;

                var wo2Id = Guid.Parse("99999999-9999-9999-9999-999999999992");

                var wo2RepairTasks = new List<WorkOrderRepairTasks>
                    {
                        WorkOrderRepairTasks.Create(
                            workOrderId: wo2Id,
                            repairTaskId: batteryRepairTask.Id,
                            time: batteryRepairTask.TimeEstimated,
                            cost: batteryRepairTask.CostEstimated
                        ).Value
                    };

                var workOrder2 = WorkOrder.Create(
                    id: wo2Id,
                    vehicleId: vehicle2.Id,
                    customerId: vehicle2.CustomerId,
                    employeeId: tech2Id,
                    startTime: DateTime.Now.AddDays(2),
                    bay: Bay.B,
                    repairTasks: wo2RepairTasks
                ).Value;

                _context.WorkOrders.AddRange([workOrder1, workOrder2]);
            }

            await _context.SaveChangesAsync();
        }

        public async Task SeedAsync()
        {
            try
            {
                await TryToSeedAsync();
            }catch(Exception ex)
            {
                _logger.LogError(ex, "An Error occured while trying to seed data");
                throw;
            }
        }
    }

    public static class InitializerExtensions
    {
        public static async Task InitilizedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDataInitilizer>();

            await initialiser.InitilizeAsync();

            await initialiser.SeedAsync();
        }
    }
}

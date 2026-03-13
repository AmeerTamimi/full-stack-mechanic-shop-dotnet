using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using GOATY.Domain.Employees.Enums;

namespace GOATY.Tests.Common.Employees
{
    public static class TechnicianFactory
    {
        public static Result<Employee> Create(Guid? id = null,
                                              string? firstName = null,
                                              string? lastName = null,
                                              Role? role = null)
        {
            return Employee.Create(
                id ?? Guid.NewGuid(),
                firstName ?? "First Name",
                lastName ?? "Last Name",
                role ?? Role.Technician);
        }
    }
}
using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees.Enums;

namespace GOATY.Domain.Employees
{
    public sealed class Employee : AuditableEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; } = $"";
        public Role Role { get; set; }

        public static Result<Employee> Create(Guid id,
                                              string firstName,
                                              string lastName,
                                              Role role
                                             )
        {

            if (string.IsNullOrWhiteSpace(firstName))
            {
                return EmployeeErrors.InvalidFirstName;
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return EmployeeErrors.InvalidLastName;
            }
            if (!Enum.IsDefined(typeof(Role), role))
            {
                return EmployeeErrors.InvalidRole;
            }

            return new Employee
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Role = role
            };
        }

        public static Result<Updated> Update(Employee employee,
                                              string firstName,
                                              string lastName,
                                              Role role
                                             )
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return EmployeeErrors.InvalidFirstName;
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return EmployeeErrors.InvalidLastName;
            }

            if (!Enum.IsDefined(typeof(Role), role))
            {
                return EmployeeErrors.InvalidRole;
            }

            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.Role = role;

            return Result.Updated;
        }
    }
}

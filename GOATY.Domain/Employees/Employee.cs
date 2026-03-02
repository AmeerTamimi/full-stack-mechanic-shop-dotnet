using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees.Enums;

namespace GOATY.Domain.Employees
{
    public sealed class Employee : AuditableEntity
    {
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string? FullName { get; private set; } = $"";
        public string? Email { get; private set; }
        public string? PasswordHash { get; private set; }
        public Role Role { get; private set; }

        private Employee() { }
        private Employee(
            Guid id,
            string firstName,
            string lastName,
            Role role)
            : base(id)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }
        public static Result<Employee> Create(Guid id,
                                              string firstName,
                                              string lastName,
                                              Role role
                                             )
        {
            if(id == Guid.Empty)
            {
                return EmployeeErrors.InvalidId;
            }
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

            return new Employee(id , firstName , lastName , role);

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

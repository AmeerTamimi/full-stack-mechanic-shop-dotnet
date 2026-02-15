using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Employees;
using GOATY.Domain.Employees.Enums;

namespace GOATY.Domain.UnitTests.Employees
{
    public sealed class EmployeeTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            var firstName = "First Name";
            var lastName = "Last Name";
            var role = Role.Labor;

            var result = Employee.Create(id, firstName, lastName, role);

            var actual = result.Value;
            var expected = new Employee
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Role = role
            };

            Assert.True(result.IsSuccess);
            Assert.Equivalent(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidId_ShouldFail()
        {
            var id = Guid.Empty; // Empty id
            var firstName = "First Name";
            var lastName = "Last Name";
            var role = Role.Labor;

            var result = Employee.Create(id, firstName, lastName, role);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidId;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual , expected);
        }

        [Fact]
        public void Create_WithInvalidFirstName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var firstName = "";
            string lastName = "Last Name";
            var role = Role.Labor;

            var result = Employee.Create(id, firstName, lastName, role);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidFirstName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidLastName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var firstName = "First Name";
            string lastName = null!;
            var role = Role.Labor;

            var result = Employee.Create(id, firstName, lastName, role);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidLastName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidRole_ShouldFail()
        {
            var id = Guid.NewGuid();
            var firstName = "First Name";
            var lastName = "Last Name";
            var role = (Role)5; // invalid Role

            var result = Employee.Create(id, firstName, lastName, role);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidRole;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            var employee = new Employee
            {
                Id = id,
                FirstName = "First Name",
                LastName = "Last Name",
                Role = Role.Labor
            };

            var newFirstName = "New First Name";
            var newLastName = "New Last Name";
            var newRole = Role.Manager;

            var result = Employee.Update(employee, newFirstName, newLastName, newRole);

            var actual = result.Value;
            var expected = new Employee
            {
                Id = id,
                FirstName = "New First Name",
                LastName = "New Last Name",
                Role = Role.Manager
            };

            Assert.True(result.IsSuccess);
            Assert.Equivalent(actual, expected);
        }

        [Fact]
        public void Update_WithInvalidFirstName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var employee = new Employee
            {
                Id = id,
                FirstName = "First Name",
                LastName = "Last Name",
                Role = Role.Labor
            };

            var newFirstName = "";
            var newLastName = "New Last Name";
            var newRole = Role.Manager;

            var result = Employee.Update(employee, newFirstName, newLastName, newRole);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidFirstName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithInvalidLastName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var employee = new Employee
            {
                Id = id,
                FirstName = "First Name",
                LastName = "Last Name",
                Role = Role.Labor
            };

            var newFirstName = "New First Name";
            string newLastName = null!;
            var newRole = Role.Manager;

            var result = Employee.Update(employee, newFirstName, newLastName, newRole);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidLastName;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Update_WithInvalidRole_ShouldFail()
        {
            var id = Guid.NewGuid();
            var employee = new Employee
            {
                Id = id,
                FirstName = "First Name",
                LastName = "Last Name",
                Role = Role.Labor
            };

            var newFirstName = "New First Name";
            var newLastName = "New Last Name";
            var newRole = (Role) 5;

            var result = Employee.Update(employee, newFirstName, newLastName, newRole);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidRole;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }
    }
}

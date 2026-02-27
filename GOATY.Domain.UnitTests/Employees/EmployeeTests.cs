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
            var role = Role.Technician;

            var result = Employee.Create(id, firstName, lastName, role);

            var actual = result.Value;

            Assert.True(result.IsSuccess);
            Assert.Equal(id, actual.Id);
            Assert.Equal(firstName, actual.FirstName);
            Assert.Equal(lastName, actual.LastName);
            Assert.Equal(role, actual.Role);
        }

        [Fact]
        public void Create_WithInvalidId_ShouldFail()
        {
            var id = Guid.Empty;
            var firstName = "First Name";
            var lastName = "Last Name";
            var role = Role.Technician;

            var result = Employee.Create(id, firstName, lastName, role);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidId;

            Assert.False(result.IsSuccess);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void Create_WithInvalidFirstName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var firstName = "";
            string lastName = "Last Name";
            var role = Role.Technician;

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
            var role = Role.Technician;

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
            var role = (Role)5;

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
            var employee = Employee.Create(id, "First Name", "Last Name", Role.Technician).Value;

            var newFirstName = "New First Name";
            var newLastName = "New Last Name";
            var newRole = Role.Manager;

            var result = Employee.Update(employee, newFirstName, newLastName, newRole);

            var actual = employee;

            Assert.True(result.IsSuccess);
            Assert.Equal(id, actual.Id);
            Assert.Equal("New First Name", actual.FirstName);
            Assert.Equal("New Last Name", actual.LastName);
            Assert.Equal(Role.Manager, actual.Role);
        }

        [Fact]
        public void Update_WithInvalidFirstName_ShouldFail()
        {
            var id = Guid.NewGuid();
            var employee = Employee.Create(id, "First Name", "Last Name", Role.Technician).Value;

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
            var employee = Employee.Create(id, "First Name", "Last Name", Role.Technician).Value;

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
            var employee = Employee.Create(id, "First Name", "Last Name", Role.Technician).Value;

            var newFirstName = "New First Name";
            var newLastName = "New Last Name";
            var newRole = (Role)5;

            var result = Employee.Update(employee, newFirstName, newLastName, newRole);

            var actual = result.Error;
            var expected = EmployeeErrors.InvalidRole;

            Assert.False(result.IsSuccess);
            Assert.Equivalent(actual, expected);
        }
    }
}
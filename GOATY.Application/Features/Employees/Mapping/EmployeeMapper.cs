using GOATY.Application.Features.Employees.DTOs;
using GOATY.Domain.Employees;

namespace GOATY.Application.Features.Employees.Mapping
{
    public static class EmployeeMapper
    {
        public static EmployeeDto ToDto(this Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Role = employee.Role
            };
        }

        public static List<EmployeeDto> ToDtos(this List<Employee> employees)
        {
            return employees.ConvertAll(emp => emp.ToDto());
        }
    }
}

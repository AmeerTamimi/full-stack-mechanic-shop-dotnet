using GOATY.Application.DTOs;
using GOATY.Domain.Employees;

namespace GOATY.Application.Mapping
{
    public static class EmployeeMapper
    {
        public static EmployeeDto ToDto(this Employee employee)
        {
            return new EmployeeDto
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Role = employee.Role
            };
        }

        public static List<EmployeeDto> ToDtos(this List<Employee> employees)
        {
            return employees.ConvertAll(emp => emp.ToDto());
        }
    }
}

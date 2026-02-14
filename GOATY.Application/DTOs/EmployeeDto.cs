using GOATY.Domain.Employees.Enums;

namespace GOATY.Application.DTOs
{
    public sealed class EmployeeDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public Role Role { get; set; }
    }
}

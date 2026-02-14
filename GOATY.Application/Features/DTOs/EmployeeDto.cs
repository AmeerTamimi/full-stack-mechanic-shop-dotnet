using GOATY.Domain.Employees.Enums;

namespace GOATY.Application.Features.DTOs
{
    public sealed class EmployeeDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        public Role Role { get; set; }
    }
}

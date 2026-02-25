using GOATY.Domain.Employees.Enums;

namespace GOATY.Contracts.Requests
{
    public sealed class EmployeeRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Role Role { get; set; }
    }
}

using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Employees
{
    public sealed class EmployeeErrors
    {
        public static Error InvalidId = Error.Validation(
                                                        code: "Employee.Id.Empty",
                                                        description: "Employee's Id Is Empty"
                                                    );

        public static Error InvalidFirstName = Error.Validation(
                                                        code: "Employee.FirstName.Invalid",
                                                        description: "Employee's First Name Is Invalid"
                                                    );

        public static Error InvalidLastName = Error.Validation(
                                                        code: "Employee.LastName.Invalid",
                                                        description: "Employee's Last Name Is Invalid"
                                                    );

        public static Error InvalidRole = Error.Validation(
                                                        code: "Employee.Role.Invalid",
                                                        description: "Employee's Role Is Invalid"
                                                    );
    }
}

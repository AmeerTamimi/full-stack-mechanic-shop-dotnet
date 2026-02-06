using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOATY.Domain.Common.Results
{
    public class Error
    {
        // this class will represent the error we send (we dont throw a whole exception and risk the system , we see an error , return to the other layer what happend wrong exactly)
        public string Code { get; }
        public string Description { get; }
        public ErrorType Type { get; }
        private Error(string code , string description , ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        public static Error Failure(string code = nameof(Failure),
                                    string description = "General Failure.")
            => new Error(code, description, ErrorType.Failure);

        public static Error Unexpected(string code = nameof(Unexpected),
                                       string description = "Unexpected Error Happend.")
            => new Error(code, description, ErrorType.Unexpected);

        public static Error Validation(string code = nameof(Validation),
                                       string description = "Validation error")
            => new(code, description, ErrorType.Validation);

        public static Error Conflict(string code = nameof(Conflict),
                                     string description = "Conflict error")
            => new(code, description, ErrorType.Conflict);

        public static Error NotFound(string code = nameof(NotFound), 
                                     string description = "Not found error")
            => new(code, description, ErrorType.NotFound);

        public static Error Unauthorized(string code = nameof(Unauthorized),
                                         string description = "Unauthorized error")
            => new(code, description, ErrorType.Unauthorized);

        public static Error Forbidden(string code = nameof(Forbidden),
                                      string description = "Forbidden error")
            => new(code, description, ErrorType.Forbidden);

        public static Error Create(int type, string code, string description)
            => new Error(code, description, (ErrorType)type);
    }
}

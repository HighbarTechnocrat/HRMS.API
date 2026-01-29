using System.Net;

namespace HRMS.API.Exceptions
{
    public class ValidationException : ApiException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("One or more validation errors occurred", HttpStatusCode.BadRequest, "VALIDATION_ERROR")
        {
            Errors = errors;
        }
    }
}

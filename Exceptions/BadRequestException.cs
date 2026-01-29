using System.Net;

namespace HRMS.API.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest, "BAD_REQUEST")
        {
        }
    }
}

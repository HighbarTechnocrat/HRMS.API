using System.Net;

namespace HRMS.API.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message)
            : base(message, HttpStatusCode.Unauthorized, "UNAUTHORIZED")
        {
        }
    }
}

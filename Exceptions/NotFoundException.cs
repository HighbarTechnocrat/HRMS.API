using System.Net;

namespace HRMS.API.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound, "NOT_FOUND")
        {
        }
    }
}

using System.Net;

namespace Application.Errors
{
    public class RestException : Exception
    {
        public RestException(HttpStatusCode httpStatusCode, object? errors = null)
        {
            HttpStatusCode = httpStatusCode;
            Errors = errors;
        }

        public HttpStatusCode HttpStatusCode { get; }

        public object? Errors { get; }
    }
}

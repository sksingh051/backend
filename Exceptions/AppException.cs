using System.Net;

namespace Phase_07_Poc_01.Exceptions
{
    /// <summary>
    /// Base application exception containing HTTP status code and optional structured errors.
    /// </summary>
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public object? Errors { get; }

        public AppException(string message, int statusCode = (int)HttpStatusCode.BadRequest, object? errors = null) 
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

        public AppException(string message, HttpStatusCode statusCode, object? errors = null) 
            : base(message)
        {
            StatusCode = (int)statusCode;
            Errors = errors;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) 
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message, object? errors = null) 
            : base(message, HttpStatusCode.BadRequest, errors)
        {
        }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "You are not authorized to perform this action.") 
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "You do not have permission to access this resource.") 
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message) 
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }
}

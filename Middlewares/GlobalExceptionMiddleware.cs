using System.Net;
using System.Security.Claims;
using Phase_07_Poc_01.DTO;
using Phase_07_Poc_01.Exceptions;
using Phase_07_Poc_01.Helper;

namespace Phase_07_Poc_01.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message, errors, logLevel) = ResolveException(exception);

            LogException(context, exception, statusCode, message, logLevel);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, cannot write exception response to client.");
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = ApiResponseResult<object>.FailureResponse(message, errors);

            await context.Response.WriteAsJsonAsync(response);
        }

        private (int StatusCode, string Message, object? Errors, LogLevel LogLevel) ResolveException(Exception exception)
        {
            return exception switch
            {
                AppException appEx => (
                    appEx.StatusCode,
                    appEx.Message,
                    appEx.Errors,
                    appEx.StatusCode >= 500 ? LogLevel.Error : LogLevel.Warning
                ),

                FluentValidation.ValidationException valEx => (
                    (int)HttpStatusCode.BadRequest,
                    "Validation failed.",
                    valEx.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }),
                    LogLevel.Warning
                ),

                KeyNotFoundException knfEx => (
                    (int)HttpStatusCode.NotFound,
                    !string.IsNullOrWhiteSpace(knfEx.Message) ? knfEx.Message : "The requested resource was not found.",
                    null,
                    LogLevel.Warning
                ),

                UnauthorizedAccessException unauthEx => (
                    (int)HttpStatusCode.Unauthorized,
                    !string.IsNullOrWhiteSpace(unauthEx.Message) ? unauthEx.Message : ResponseMessages.Unauthorized,
                    null,
                    LogLevel.Warning
                ),

                BadHttpRequestException badReqEx => (
                    badReqEx.StatusCode != 0 ? badReqEx.StatusCode : (int)HttpStatusCode.BadRequest,
                    badReqEx.Message,
                    null,
                    LogLevel.Warning
                ),

                ArgumentException argEx => (
                    (int)HttpStatusCode.BadRequest,
                    argEx.Message,
                    null,
                    LogLevel.Warning
                ),

                InvalidOperationException invOpEx => (
                    (int)HttpStatusCode.BadRequest,
                    invOpEx.Message,
                    null,
                    LogLevel.Warning
                ),

                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    _env.IsDevelopment()
                        ? $"{exception.Message} ({exception.GetType().Name})"
                        : ResponseMessages.Failure,
                    _env.IsDevelopment()
                        ? new { StackTrace = exception.StackTrace, Source = exception.Source }
                        : null,
                    LogLevel.Error
                )
            };
        }

        private void LogException(HttpContext context, Exception exception, int statusCode, string message, LogLevel logLevel)
        {
            var httpMethod = context.Request.Method;
            var requestPath = context.Request.Path;
            var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
            var traceId = context.TraceIdentifier;
            var userId = context.User?.FindFirst("id")?.Value
                         ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? "Anonymous";
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var exceptionType = exception.GetType().Name;

            if (logLevel >= LogLevel.Error)
            {
                _logger.LogError(
                    exception,
                    "[ERROR {StatusCode}] {HttpMethod} {RequestPath}{QueryString} | TraceId: {TraceId} | User: {UserId} | IP: {ClientIp} | Exception: {ExceptionType} | Message: {ErrorMessage}",
                    statusCode, httpMethod, requestPath, queryString, traceId, userId, clientIp, exceptionType, message
                );
            }
            else
            {
                _logger.LogWarning(
                    "[CLIENT {StatusCode}] {HttpMethod} {RequestPath}{QueryString} | TraceId: {TraceId} | User: {UserId} | IP: {ClientIp} | Exception: {ExceptionType} | Reason: {ErrorMessage}",
                    statusCode, httpMethod, requestPath, queryString, traceId, userId, clientIp, exceptionType, message
                );
            }
        }
    }
}

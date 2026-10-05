namespace Phase_07_Poc_01.Filters;

using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

public class LogActionFilter : IAsyncActionFilter
{
    private readonly ILogger<LogActionFilter> _logger;

    public LogActionFilter(ILogger<LogActionFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        //Before Action 
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        var httpMethod = context.HttpContext.Request.Method;
        var requestPath = context.HttpContext.Request.Path;
        var userId = context.HttpContext.User.FindFirst("id")?.Value ?? "anonymous";

        _logger.LogInformation(
            "[START] {HttpMethod} {RequestPath} | Controller: {Controller} | Action: {Action} | User: {UserId}",
            httpMethod, requestPath, controllerName, actionName, userId);

        //start timer
        var stopwatch = Stopwatch.StartNew();

        //Execute Action
        var executedContext = await next();

        //stop timer
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        //After Action
        if (executedContext.Exception != null && !executedContext.ExceptionHandled)
        {
            // action threw unhandled exception
            _logger.LogCritical(
                executedContext.Exception,
                "[FAILED] {HttpMethod} {RequestPath} | Controller: {Controller} | Action: {Action} | Duration: {ElapsedMs}ms | Error: {ErrorMessage}",
                httpMethod, requestPath, controllerName, actionName, elapsedMs,
                executedContext.Exception.Message);
        }
        else
        {
            // get response status code
            var statusCode = context.HttpContext.Response.StatusCode;

            // determine log level based on status code
            if (statusCode >= 500)
            {
                _logger.LogCritical(
                    "[END] {HttpMethod} {RequestPath} | Controller: {Controller} | Action: {Action} | Status: {StatusCode} | Duration: {ElapsedMs}ms",
                    httpMethod, requestPath, controllerName, actionName, statusCode, elapsedMs);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "[END] {HttpMethod} {RequestPath} | Controller: {Controller} | Action: {Action} | Status: {StatusCode} | Duration: {ElapsedMs}ms",
                    httpMethod, requestPath, controllerName, actionName, statusCode, elapsedMs);
            }
            else
            {
                _logger.LogInformation(
                    "[END] {HttpMethod} {RequestPath} | Controller: {Controller} | Action: {Action} | Status: {StatusCode} | Duration: {ElapsedMs}ms",
                    httpMethod, requestPath, controllerName, actionName, statusCode, elapsedMs);
            }
        }

        //performance warning
        if (elapsedMs > 1000)
        {
            _logger.LogWarning(
                "[SLOW] {HttpMethod} {RequestPath} | Action: {Action} took {ElapsedMs}ms — exceeds 1000ms threshold",
                httpMethod, requestPath, actionName, elapsedMs);
        }
    }
}
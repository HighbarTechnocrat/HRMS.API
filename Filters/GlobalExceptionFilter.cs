// Filters/GlobalExceptionFilter.cs
using HRMS.API.DTOs;
using HRMS.API.Exceptions;
using HRMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace HRMS.API.Filters
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger,IServiceProvider serviceProvider,IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            // Get controller and action names
            var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Unknown";
            var actionName = context.RouteData.Values["action"]?.ToString() ?? "Unknown";
            var exception = context.Exception;

            // Create scope to resolve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                var errorLogService = scope.ServiceProvider.GetRequiredService<IErrorLogService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                try
                {
                    // Log exception to database and email
                    await LogExceptionAsync(context, errorLogService, emailService, controllerName, actionName, exception);
                }
                catch (Exception loggingEx)
                {
                    // Fallback logging if error logging fails
                    _logger.LogError(loggingEx, "Failed to log exception in GlobalExceptionFilter");
                }
            }

            // Create proper HTTP response
            SetExceptionResponse(context, exception);

            // Mark exception as handled
            context.ExceptionHandled = true;
        }

        private async Task LogExceptionAsync(
            ExceptionContext context,
            IErrorLogService errorLogService,
            IEmailService emailService,
            string controllerName,
            string actionName,
            Exception exception)
        {
            var httpContext = context.HttpContext;

            // Extract user identifier
            string userId = httpContext.User?.FindFirst("UserId")?.Value
                           ?? httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                           ?? "Anonymous";

            // Log to database
            await errorLogService.LogErrorAsync(
                controllerName: controllerName,
                actionName: actionName,
                errorMessage: exception.Message,
                stackTrace: exception.StackTrace,
                requestPath: httpContext.Request.Path,
                requestMethod: httpContext.Request.Method,
                userId: userId,
                errorLevel: "Error"
            );

            // Check if email notification is enabled
            var enableErrorEmail = _configuration.GetValue<bool>("ErrorNotification:EnableEmail", false);

            if (enableErrorEmail)
            {
                try
                {
                    await emailService.SendErrorNotificationAsync(
                        controllerName: controllerName,
                        actionName: actionName,
                        errorMessage: exception.Message,
                        stackTrace: exception.StackTrace
                    );
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send error notification email");

                    // Log email sending failure to database
                    await errorLogService.LogErrorAsync(
                        controllerName: controllerName,
                        actionName: actionName,
                        errorMessage: $"Failed to send error email: {emailEx.Message}",
                        errorLevel: "Warning"
                    );
                }
            }
        }

        private void SetExceptionResponse(ExceptionContext context, Exception exception)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = context.HttpContext.Request.Path,
                Title = "An error occurred while processing your request",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            // Customize response based on exception type
            if (exception is ApiException apiException)
            {
                problemDetails.Status = (int)apiException.StatusCode;
                problemDetails.Title = apiException.ErrorCode ?? "API Error";
                problemDetails.Detail = apiException.Message;

                // For validation exceptions, add additional details
                if (exception is ValidationException validationException)
                {
                    problemDetails.Extensions["errors"] = validationException.Errors;
                }
            }
            else
            {
                switch (exception)
                {
                    case UnauthorizedAccessException:
                        problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                        problemDetails.Title = "Unauthorized";
                        problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                        break;

                    case KeyNotFoundException:
                    case FileNotFoundException:
                        problemDetails.Status = (int)HttpStatusCode.NotFound;
                        problemDetails.Title = "Resource not found";
                        problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                        break;

                    case ArgumentException:
                    case InvalidOperationException:
                        problemDetails.Status = (int)HttpStatusCode.BadRequest;
                        problemDetails.Title = "Bad request";
                        problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                        break;

                    case NotImplementedException:
                        problemDetails.Status = (int)HttpStatusCode.NotImplemented;
                        problemDetails.Title = "Not implemented";
                        problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.2";
                        break;

                    case TimeoutException:
                        problemDetails.Status = (int)HttpStatusCode.RequestTimeout;
                        problemDetails.Title = "Request timeout";
                        problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.7";
                        break;

                    default:
                        // For security, don't expose internal details in production
                        if (context.HttpContext.RequestServices.GetService<IWebHostEnvironment>().IsProduction())
                        {
                            problemDetails.Detail = "An internal server error occurred. Please contact support.";
                        }
                        break;
                }
            }

            // Set the response
            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };

            // Also set response content type
            context.HttpContext.Response.ContentType = "application/problem+json";


        }
    }
}
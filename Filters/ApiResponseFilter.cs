// Filters/ApiResponseFilter.cs
using HRMS.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRMS.API.Filters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // You can add pre-execution logic here if needed
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                // Exception will be handled by GlobalExceptionFilter
                return;
            }

            // Only process successful results
            if (context.Result is ObjectResult objectResult)
            {
                // If the result is already an ApiResponse, leave it as is
                if (objectResult.Value is ApiResponse<object>)
                {
                    return;
                }

                // Wrap successful responses in ApiResponse
                var apiResponse = new ApiResponse<object>
                {
                    Success = true,
                    Message = "Request successful",
                    Data = objectResult.Value
                };

                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
            else if (context.Result is EmptyResult emptyResult)
            {
                // Handle empty results (like 204 No Content)
                var apiResponse = new ApiResponse<object>
                {
                    Success = true,
                    Message = "Request completed successfully",
                    Data = null
                };

                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = 200
                };
            }
        }
    }
}
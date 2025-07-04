// <copyright file="ErrorHandlingMiddleware.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace HotshotLogistics.Api.Middleware
{
    /// <summary>
    /// Middleware for centralized error handling.
    /// </summary>
    public class ErrorHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invokes the middleware to handle errors.
        /// </summary>
        /// <param name="context">The function context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An unhandled exception occurred during function execution. FunctionName: {FunctionName}, InvocationId: {InvocationId}", 
                    context.FunctionDefinition.Name, context.InvocationId);

                await this.HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles the exception and creates an appropriate HTTP response.
        /// </summary>
        /// <param name="context">The function context.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task HandleExceptionAsync(FunctionContext context, Exception exception)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();
            if (httpRequestData == null)
            {
                return;
            }

            var response = httpRequestData.CreateResponse();
            var errorResponse = CreateErrorResponse(exception);

            response.StatusCode = errorResponse.StatusCode;
            await response.WriteStringAsync(JsonSerializer.Serialize(errorResponse.Error, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

            context.GetInvocationResult().Value = response;
        }

        /// <summary>
        /// Creates an error response based on the exception type.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>An error response with appropriate status code and message.</returns>
        private static (HttpStatusCode StatusCode, object Error) CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    new
                    {
                        message = "Validation failed",
                        errors = validationEx.Errors.Select(e => new { property = e.PropertyName, message = e.ErrorMessage })
                    }
                ),
                ArgumentNullException => (
                    HttpStatusCode.BadRequest,
                    new { message = "Required data is missing" }
                ),
                ArgumentException => (
                    HttpStatusCode.BadRequest,
                    new { message = "Invalid request data" }
                ),
                KeyNotFoundException => (
                    HttpStatusCode.NotFound,
                    new { message = "The requested resource was not found" }
                ),
                InvalidOperationException => (
                    HttpStatusCode.Conflict,
                    new { message = "The operation could not be completed due to a conflict" }
                ),
                UnauthorizedAccessException => (
                    HttpStatusCode.Unauthorized,
                    new { message = "Access denied" }
                ),
                _ => (
                    HttpStatusCode.InternalServerError,
                    new { message = "An internal server error occurred. Please try again later." }
                )
            };
        }
    }
}
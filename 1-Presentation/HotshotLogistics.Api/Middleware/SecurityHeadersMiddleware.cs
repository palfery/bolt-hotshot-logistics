// <copyright file="SecurityHeadersMiddleware.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace HotshotLogistics.Api.Middleware
{
    /// <summary>
    /// Middleware to add security headers to HTTP responses.
    /// </summary>
    public class SecurityHeadersMiddleware : IFunctionsWorkerMiddleware
    {
        /// <summary>
        /// Invokes the middleware to add security headers.
        /// </summary>
        /// <param name="context">The function context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            await next(context);

            // Add security headers to HTTP responses
            var httpReqData = await context.GetHttpRequestDataAsync();
            if (httpReqData != null)
            {
                var httpResponseData = context.GetHttpResponseData();
                if (httpResponseData != null)
                {
                    AddSecurityHeaders(httpResponseData);
                }
            }
        }

        /// <summary>
        /// Adds security headers to the HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response data.</param>
        private static void AddSecurityHeaders(HttpResponseData response)
        {
            // Prevent XSS attacks
            response.Headers.Add("X-Content-Type-Options", "nosniff");
            
            // Prevent clickjacking
            response.Headers.Add("X-Frame-Options", "DENY");
            
            // Enable browser XSS protection
            response.Headers.Add("X-XSS-Protection", "1; mode=block");
            
            // Content Security Policy
            response.Headers.Add("Content-Security-Policy", 
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' https:; " +
                "connect-src 'self' https:; " +
                "frame-ancestors 'none';");
            
            // Referrer Policy
            response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            
            // Permissions Policy
            response.Headers.Add("Permissions-Policy", 
                "accelerometer=(), " +
                "camera=(), " +
                "geolocation=(), " +
                "gyroscope=(), " +
                "magnetometer=(), " +
                "microphone=(), " +
                "payment=(), " +
                "usb=()");

            // Remove server information
            response.Headers.Remove("Server");
        }
    }
}
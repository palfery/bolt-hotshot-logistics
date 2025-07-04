// Security Headers and Rate Limiting Example for Hotshot Logistics
// Add this configuration to your Program.cs file

using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace HotshotLogistics.Api
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Existing configuration...
                })
                .ConfigureServices((context, services) =>
                {
                    // Add Rate Limiting
                    services.AddRateLimiter(options =>
                    {
                        // General API rate limiting
                        options.AddFixedWindowLimiter("ApiPolicy", opt =>
                        {
                            opt.PermitLimit = 100;
                            opt.Window = TimeSpan.FromMinutes(1);
                            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                            opt.QueueLimit = 10;
                        });

                        // Strict rate limiting for authentication endpoints
                        options.AddFixedWindowLimiter("AuthPolicy", opt =>
                        {
                            opt.PermitLimit = 5;
                            opt.Window = TimeSpan.FromMinutes(1);
                            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                            opt.QueueLimit = 2;
                        });

                        // Sliding window for data modification endpoints
                        options.AddSlidingWindowLimiter("DataModificationPolicy", opt =>
                        {
                            opt.PermitLimit = 50;
                            opt.Window = TimeSpan.FromMinutes(1);
                            opt.SegmentsPerWindow = 6; // 10-second segments
                            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                            opt.QueueLimit = 5;
                        });

                        // Per-user rate limiting
                        options.AddTokenBucketLimiter("PerUserPolicy", opt =>
                        {
                            opt.TokenLimit = 100;
                            opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                            opt.QueueLimit = 5;
                            opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                            opt.TokensPerPeriod = 20;
                        });

                        // Global rejection response
                        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                        
                        options.OnRejected = async (context, token) =>
                        {
                            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                            await context.HttpContext.Response.WriteAsync(
                                "Too many requests. Please try again later.", token);
                        };
                    });

                    // Add CORS
                    services.AddCors(options =>
                    {
                        options.AddPolicy("ProductionPolicy", policy =>
                        {
                            policy.WithOrigins(
                                "https://yourdomain.com",
                                "https://app.yourdomain.com",
                                "https://admin.yourdomain.com"
                            )
                            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                            .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                            .AllowCredentials()
                            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                        });

                        options.AddPolicy("DevelopmentPolicy", policy =>
                        {
                            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowCredentials();
                        });
                    });

                    // Existing services...
                    services.AddAuthentication(/* ... */);
                    services.AddAuthorization(/* ... */);
                    services.AddControllers();
                })
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseKestrel(options =>
                    {
                        // Configure Kestrel server security
                        options.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
                        });
                    });
                })
                .Build();

            // Configure security middleware pipeline
            var app = host.Services.GetRequiredService<IHost>();
            
            ConfigureSecurityMiddleware(app);
            
            host.Run();
        }

        private static void ConfigureSecurityMiddleware(IHost host)
        {
            var app = host as WebApplication ?? throw new InvalidOperationException("Host is not a WebApplication");

            // Security headers middleware (add early in pipeline)
            app.Use(async (context, next) =>
            {
                // Security headers
                var headers = context.Response.Headers;
                
                // Prevent MIME sniffing
                headers["X-Content-Type-Options"] = "nosniff";
                
                // Prevent clickjacking
                headers["X-Frame-Options"] = "DENY";
                
                // XSS protection
                headers["X-XSS-Protection"] = "1; mode=block";
                
                // HSTS (HTTP Strict Transport Security)
                headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
                
                // Content Security Policy
                headers["Content-Security-Policy"] = string.Join("; ", new[]
                {
                    "default-src 'self'",
                    "script-src 'self'",
                    "style-src 'self' 'unsafe-inline'",
                    "img-src 'self' data: https:",
                    "font-src 'self'",
                    "connect-src 'self'",
                    "media-src 'self'",
                    "object-src 'none'",
                    "child-src 'none'",
                    "frame-ancestors 'none'",
                    "base-uri 'self'",
                    "form-action 'self'"
                });
                
                // Referrer Policy
                headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                
                // Permissions Policy
                headers["Permissions-Policy"] = string.Join(", ", new[]
                {
                    "camera=()",
                    "microphone=()",
                    "geolocation=()",
                    "payment=()",
                    "usb=()",
                    "magnetometer=()",
                    "accelerometer=()",
                    "gyroscope=()"
                });
                
                // Remove server header
                headers.Remove("Server");
                
                await next.Invoke();
            });

            // HTTPS redirection
            app.UseHttpsRedirection();

            // Rate limiting
            app.UseRateLimiter();

            // CORS (if needed, configure carefully)
            var environment = app.Services.GetRequiredService<IWebHostEnvironment>();
            if (environment.IsDevelopment())
            {
                app.UseCors("DevelopmentPolicy");
            }
            else
            {
                app.UseCors("ProductionPolicy");
            }

            // Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Controllers
            app.UseRouting();
            app.MapControllers();
        }
    }
}

// Security middleware components
namespace HotshotLogistics.Api.Middleware
{
    // Custom security logging middleware
    public class SecurityLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityLoggingMiddleware> _logger;

        public SecurityLoggingMiddleware(RequestDelegate next, ILogger<SecurityLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log security-relevant request information
            var requestInfo = new
            {
                Method = context.Request.Method,
                Path = context.Request.Path,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                IP = GetClientIP(context),
                UserId = context.User?.Identity?.Name ?? "Anonymous"
            };

            // Check for suspicious patterns
            if (IsSuspiciousRequest(context))
            {
                _logger.LogWarning("Suspicious request detected: {@RequestInfo}", requestInfo);
            }

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                // Log completed request
                _logger.LogInformation("Request completed: {@RequestInfo} Status: {StatusCode} Duration: {Duration}ms",
                    requestInfo, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
                
                // Log security events
                if (context.Response.StatusCode == 401)
                {
                    _logger.LogWarning("Unauthorized access attempt: {@RequestInfo}", requestInfo);
                }
                else if (context.Response.StatusCode == 403)
                {
                    _logger.LogWarning("Forbidden access attempt: {@RequestInfo}", requestInfo);
                }
            }
        }

        private static string GetClientIP(HttpContext context)
        {
            // Check for forwarded IP addresses (when behind a proxy)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIP = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIP))
            {
                return realIP;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private static bool IsSuspiciousRequest(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLowerInvariant();
            var userAgent = context.Request.Headers["User-Agent"].ToString().ToLowerInvariant();

            // Check for common attack patterns
            var suspiciousPatterns = new[]
            {
                "sql", "union", "select", "insert", "delete", "drop", "script",
                "javascript:", "vbscript:", "onload", "onerror", "../", "..\\",
                "cmd.exe", "powershell", "/etc/passwd", "wp-admin", "admin.php"
            };

            return suspiciousPatterns.Any(pattern => 
                path.Contains(pattern) || 
                context.Request.QueryString.ToString().ToLowerInvariant().Contains(pattern)) ||
                string.IsNullOrEmpty(userAgent) ||
                userAgent.Contains("bot") ||
                userAgent.Contains("crawler");
        }
    }

    // Request size limiting middleware
    public class RequestSizeLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly long _maxRequestSize;

        public RequestSizeLimitMiddleware(RequestDelegate next, long maxRequestSize = 1024 * 1024) // 1MB default
        {
            _next = next;
            _maxRequestSize = maxRequestSize;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentLength.HasValue && 
                context.Request.ContentLength.Value > _maxRequestSize)
            {
                context.Response.StatusCode = 413; // Payload Too Large
                await context.Response.WriteAsync("Request size exceeds maximum allowed size");
                return;
            }

            await _next(context);
        }
    }

    // IP whitelist middleware (for admin endpoints)
    public class IPWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IList<string> _allowedIPs;

        public IPWhitelistMiddleware(RequestDelegate next, IList<string> allowedIPs)
        {
            _next = next;
            _allowedIPs = allowedIPs ?? new List<string>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIP = GetClientIP(context);
            
            if (!IsIPAllowed(clientIP))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access denied from this IP address");
                return;
            }

            await _next(context);
        }

        private static string GetClientIP(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "";
        }

        private bool IsIPAllowed(string clientIP)
        {
            if (string.IsNullOrEmpty(clientIP) || !_allowedIPs.Any())
                return false;

            return _allowedIPs.Contains(clientIP) || 
                   _allowedIPs.Any(allowedIP => IsIPInRange(clientIP, allowedIP));
        }

        private static bool IsIPInRange(string clientIP, string allowedRange)
        {
            // Simple CIDR checking - you may want to use a more robust library
            if (!allowedRange.Contains('/'))
                return clientIP == allowedRange;

            // This is a simplified implementation
            // For production, consider using a library like IPNetwork2
            return false;
        }
    }
}

// Controller examples with rate limiting
namespace HotshotLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("ApiPolicy")] // Apply default rate limiting
    public class JobAssignmentsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetAssignments()
        {
            // Implementation...
            return Ok();
        }

        [HttpPost]
        [EnableRateLimiting("DataModificationPolicy")] // Override with stricter policy
        public async Task<ActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
        {
            // Implementation...
            return Ok();
        }

        [HttpDelete("{id}")]
        [EnableRateLimiting("DataModificationPolicy")]
        [DisableRateLimiting] // Disable for specific actions if needed
        public async Task<ActionResult> DeleteAssignment(string id)
        {
            // Implementation...
            return Ok();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("AuthPolicy")] // Strict rate limiting for auth
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            // Implementation...
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            // Implementation...
            return Ok();
        }
    }
}

// Extension method to add all security middleware
public static class SecurityMiddlewareExtensions
{
    public static IServiceCollection AddSecurityMiddleware(this IServiceCollection services)
    {
        services.AddTransient<SecurityLoggingMiddleware>();
        services.AddTransient<RequestSizeLimitMiddleware>();
        services.AddTransient<IPWhitelistMiddleware>();
        
        return services;
    }

    public static IApplicationBuilder UseSecurityMiddleware(this IApplicationBuilder app, 
        IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Add security logging
        app.UseMiddleware<SecurityLoggingMiddleware>();

        // Add request size limiting
        app.UseMiddleware<RequestSizeLimitMiddleware>();

        // Add IP whitelisting for admin endpoints (if configured)
        var allowedIPs = configuration.GetSection("Security:AllowedIPs").Get<List<string>>();
        if (allowedIPs?.Any() == true)
        {
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/admin"),
                appBuilder => appBuilder.UseMiddleware<IPWhitelistMiddleware>(allowedIPs));
        }

        return app;
    }
}
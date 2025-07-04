// Authentication Example Configuration for Hotshot Logistics
// Add this to your Program.cs file

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                    // Add Authentication Services
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        // For Azure AD integration
                        options.Authority = $"https://login.microsoftonline.com/{context.Configuration["AzureAd:TenantId"]}";
                        options.Audience = context.Configuration["AzureAd:ClientId"];
                        
                        // Alternative: Custom JWT configuration
                        /*
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = context.Configuration["Jwt:Issuer"],
                            ValidAudience = context.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(context.Configuration["Jwt:SecretKey"]))
                        };
                        */
                    });

                    // Add Authorization Policies
                    services.AddAuthorization(options =>
                    {
                        // Admin-only actions
                        options.AddPolicy("AdminOnly", policy =>
                            policy.RequireRole("Admin"));

                        // Driver access (can access own data)
                        options.AddPolicy("DriverAccess", policy =>
                            policy.RequireRole("Driver", "Admin"));

                        // Manager access (can manage jobs and assignments)
                        options.AddPolicy("ManagerAccess", policy =>
                            policy.RequireRole("Manager", "Admin"));

                        // Dispatcher access (can assign jobs)
                        options.AddPolicy("DispatcherAccess", policy =>
                            policy.RequireRole("Dispatcher", "Manager", "Admin"));
                    });

                    // Existing services...
                    services.AddHotshotDbContext(context.Configuration);
                    services.AddHotshotRepositories();
                    services.AddApplicationServices();
                    services.AddControllers();
                })
                .Build();

            host.Run();
        }
    }
}

// Example Controller with Authentication
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotshotLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all actions
    public class JobAssignmentsController : ControllerBase
    {
        // Allow all authenticated users to view assignments
        [HttpGet]
        [Authorize(Policy = "DriverAccess")]
        public async Task<ActionResult<IEnumerable<JobAssignmentDto>>> GetAssignments()
        {
            // Implementation...
        }

        // Only managers and admins can create assignments
        [HttpPost]
        [Authorize(Policy = "ManagerAccess")]
        public async Task<ActionResult<JobAssignmentDto>> CreateAssignment(
            [FromBody] CreateAssignmentRequest request)
        {
            // Implementation...
        }

        // Only admins can delete assignments
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteAssignment(string id)
        {
            // Implementation...
        }
    }
}

// Configuration for local.settings.json (Development)
/*
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    
    // Azure AD Configuration
    "AzureAd:TenantId": "your-tenant-id",
    "AzureAd:ClientId": "your-client-id",
    
    // Alternative: Custom JWT Configuration
    "Jwt:Issuer": "https://localhost:7071",
    "Jwt:Audience": "hotshot-logistics-api",
    "Jwt:SecretKey": "your-super-secret-key-minimum-32-characters"
  }
}
*/

// Configuration for Azure App Service (Production)
/*
Set these as Application Settings in Azure Portal:

AzureAd__TenantId: your-production-tenant-id
AzureAd__ClientId: your-production-client-id

Or for custom JWT:
Jwt__Issuer: https://yourapi.azurewebsites.net
Jwt__Audience: hotshot-logistics-api
Jwt__SecretKey: (stored in Key Vault)
*/
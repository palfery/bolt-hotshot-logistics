using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Added for IHostEnvironment
using System;

namespace HotshotLogistics.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHotshotDbContext(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            string connectionString = "";
            string dbUser = null;
            string dbPassword = null;

            // Directly try to get from environment variables, assuming Program.cs has set them
            // from local.settings.json when ASPNETCORE_ENVIRONMENT is Development.
            dbUser = Environment.GetEnvironmentVariable("HSL_DBUser");
            dbPassword = Environment.GetEnvironmentVariable("HSL_DBPassword");

            if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException("HSL_DBUser or HSL_DBPassword not configured in configuration or environment variables");
            }

            connectionString = $"Server=localhost;Database=HotshotLogistics;User={dbUser};Password={dbPassword};Port=3306;SslMode=None;";
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            services.AddDbContext<HotshotDbContext>(options =>
                options.UseMySql(connectionString, serverVersion));

            return services;
        }
    }
}
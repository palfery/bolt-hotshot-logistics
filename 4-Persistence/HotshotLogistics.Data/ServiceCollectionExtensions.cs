using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HotshotLogistics.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHotshotDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Check if the MySQL connection string is configured
            var connectionString = Environment.GetEnvironmentVariable("MySqlConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MySqlConnectionString not configured");
            }

            // Specify the server version explicitly to resolve the error
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            services.AddDbContext<HotshotDbContext>(options =>
                options.UseMySql(connectionString, serverVersion));

            return services;
        }
    }
}

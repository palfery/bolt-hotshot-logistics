using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using HotshotLogistics.Data.Repositories;
using HotshotLogistics.Contracts.Repositories;

namespace HotshotLogistics.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHotshotDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                var dbUser = Environment.GetEnvironmentVariable("HSL_DBUser");
                var dbPassword = Environment.GetEnvironmentVariable("HSL_DBPassword");

                if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
                {
                    throw new InvalidOperationException("Database connection configuration is missing. Provide ConnectionStrings:DefaultConnection or HSL_DBUser/HSL_DBPassword variables.");
                }

                connectionString = $"Server=localhost;Database=hotshot_logistics;User={dbUser};Password={dbPassword};Port=3306;SslMode=None;";
            }

            var serverVersion = ServerVersion.AutoDetect(connectionString);
            services.AddDbContext<HotshotDbContext>(options =>
                options.UseMySql(connectionString, serverVersion));

            return services;
        }

        public static IServiceCollection AddHotshotRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDriverRepository, DriverRepository>();
            return services;
        }
    }
}
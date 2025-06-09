using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HotshotLogistics.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHotshotDbContext(this IServiceCollection services)
        {
            services.AddDbContext<HotshotDbContext>(options =>
                options.UseMySql(
                    Environment.GetEnvironmentVariable("MySqlConnectionString") ??
                    throw new InvalidOperationException("MySqlConnectionString not configured"),
                    ServerVersion.AutoDetect(Environment.GetEnvironmentVariable("MySqlConnectionString") ??
                    throw new InvalidOperationException("MySqlConnectionString not configured"))));
          
            return services;
        }
    }
}

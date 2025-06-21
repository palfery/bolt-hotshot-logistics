// <copyright file="ServiceCollectionExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using HotshotLogistics.Contracts.Repositories;
using HotshotLogistics.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotshotLogistics.Data
{
    /// <summary>
    /// Extension methods for configuring services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Hotshot database context to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection for chaining.</returns>
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

        /// <summary>
        /// Adds Hotshot repositories to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddHotshotRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobAssignmentRepository, JobAssignmentRepository>();
            return services;
        }
    }
}

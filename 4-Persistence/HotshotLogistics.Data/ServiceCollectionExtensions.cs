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
                var dbServer = Environment.GetEnvironmentVariable("HSL_DBServer") ?? "localhost";
                var dbName = Environment.GetEnvironmentVariable("HSL_DBName") ?? "hotshot_logistics";
                var dbUser = Environment.GetEnvironmentVariable("HSL_DBUser");
                var dbPassword = Environment.GetEnvironmentVariable("HSL_DBPassword");

                if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
                {
                    throw new InvalidOperationException("Database connection configuration is missing. Provide ConnectionStrings:DefaultConnection or HSL_DBServer/HSL_DBName/HSL_DBUser/HSL_DBPassword variables.");
                }

                connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=true;";
            }

            services.AddDbContext<HotshotDbContext>(options =>
                options.UseSqlServer(connectionString));

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

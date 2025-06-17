// <copyright file="ServiceCollectionExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data
{
    using System;
    using HotshotLogistics.Contracts.Repositories;
    using HotshotLogistics.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobAssignmentRepository, JobAssignmentRepository>();
            return services;
        }
    }
}
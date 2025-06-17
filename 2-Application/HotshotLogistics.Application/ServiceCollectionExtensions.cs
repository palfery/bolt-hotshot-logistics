using HotshotLogistics.Application.Services;
using HotshotLogistics.Contracts.Repositories;
using HotshotLogistics.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HotshotLogistics.Application;

/// <summary>
/// Extension methods for setting up services in the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IJobAssignmentService, JobAssignmentService>();

        return services;
    }
}

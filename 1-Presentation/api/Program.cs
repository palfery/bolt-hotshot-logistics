using Azure.Identity;
using HotshotLogistics.Infrastructure.Data;
using HotshotLogistics.Core.Interfaces;
using HotshotLogistics.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        // Load configuration from Azure App Configuration
        var appConfigConnectionString = Environment.GetEnvironmentVariable("AppConfigConnectionString");
        if (!string.IsNullOrEmpty(appConfigConnectionString))
        {
            config.AddAzureAppConfiguration(options =>
            {
                options.Connect(appConfigConnectionString)
                    .UseFeatureFlags()
                    .ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
            });
        }
    })
    .ConfigureServices((context, services) =>
    {
        // Configure MySQL
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        services.AddDbContext<HotshotDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // Register repositories
        services.AddScoped<IJobTypeRepository, JobTypeRepository>();
    })
    .Build();

host.Run(); 
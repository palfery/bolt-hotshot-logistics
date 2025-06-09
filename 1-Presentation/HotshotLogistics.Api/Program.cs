    using HotshotLogistics.Application.Services;
using HotshotLogistics.Contracts.Services;
using HotshotLogistics.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

var host = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var settings = config.Build();

        // Get Azure App Configuration endpoint from environment or local.settings.json
        var appConfigEndpoint = settings["AppConfig:Endpoint"];
        if (!string.IsNullOrEmpty(appConfigEndpoint))
        {
            _ = config.AddAzureAppConfiguration(options =>
            {
                _ = options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
                       // Sentinel key for refresh
                       .ConfigureRefresh(refresh =>
                       {
                           _ = refresh.Register("Sentinel", refreshAll: true)
                                  .SetRefreshInterval(TimeSpan.FromSeconds(30)); // Updated method to SetRefreshInterval
                       })
                       .Select("*");
            });
        }
    })
    .ConfigureServices((context, services) =>
    {
        // Register Azure App Configuration refresh service
        _ = services.AddAzureAppConfiguration();

        // Register DbContext
        _ = services.AddHotshotDbContext();

        // Register application services
        _ = services.AddScoped<IDriverService, DriverService>();
    })
    .Build();

host.Run();
using HotshotLogistics.Application.Services;
using HotshotLogistics.Contracts.Services;
using HotshotLogistics.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
     .ConfigureServices(services =>
    {
        // Register DbContext
        //call hotshotlogistics.data AddHotshotDbContext extension method to register the DbContext 
        services.AddHotshotDbContext();

        // Register application services
        services.AddScoped<IDriverService, DriverService>();
    })
    .Build();

host.Run();
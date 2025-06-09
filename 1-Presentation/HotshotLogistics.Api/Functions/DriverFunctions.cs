using System.Net;
using HotshotLogistics.Domain.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using HotshotLogistics.Contracts.Services;
using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Api.Functions;

public class DriverFunctions
{
    private readonly IDriverService _driverService;

    public DriverFunctions(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [Function("GetDrivers")]
    public async Task<HttpResponseData> GetDrivers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "drivers")] HttpRequestData req)
    {
        var drivers = await _driverService.GetDriversAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(drivers);
        return response;
    }

    [Function("GetDriver")]
    public async Task<HttpResponseData> GetDriver(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "drivers/{id}")] HttpRequestData req,
        int id)
    {
        var driver = await _driverService.GetDriverByIdAsync(id);
        if (driver == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"Driver with ID {id} not found");
            return notFoundResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(driver);
        return response;
    }

    [Function("CreateDriver")]
    public async Task<HttpResponseData> CreateDriver(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "drivers")] HttpRequestData req)
    {
        IDriver driver = await JsonSerializer.DeserializeAsync<IDriver>(req.Body);
        if (driver == null)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid driver data");
            return badRequestResponse;
        }

        var createdDriver = await _driverService.CreateDriverAsync(driver);
        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(createdDriver);
        return response;
    }
}
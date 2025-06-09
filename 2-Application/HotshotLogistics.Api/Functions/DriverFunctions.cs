using System.Net;
using HotshotLogistics.Core.Models;
using HotshotLogistics.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HotshotLogistics.Api.Functions;

public class DriverFunctions
{
    private readonly HotshotDbContext _dbContext;

    public DriverFunctions(HotshotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Function("GetDrivers")]
    public async Task<HttpResponseData> GetDrivers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "drivers")] HttpRequestData req)
    {
        var drivers = await _dbContext.Drivers.ToListAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(drivers);
        return response;
    }

    [Function("GetDriver")]
    public async Task<HttpResponseData> GetDriver(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "drivers/{id}")] HttpRequestData req,
        int id)
    {
        var driver = await _dbContext.Drivers.FindAsync(id);
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
        var driver = await JsonSerializer.DeserializeAsync<Driver>(req.Body);
        if (driver == null)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid driver data");
            return badRequestResponse;
        }

        driver.CreatedAt = DateTime.UtcNow;
        await _dbContext.Drivers.AddAsync(driver);
        await _dbContext.SaveChangesAsync();

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(driver);
        return response;
    }
} 
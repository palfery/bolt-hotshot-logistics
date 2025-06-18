// <copyright file="DriverFunctions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Api.Functions
{
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Contracts.Services;
    using HotshotLogistics.Domain.Models;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;

    /// <summary>
    /// Azure Functions for managing drivers.
    /// </summary>
    public class DriverFunctions
    {
        private readonly IDriverService driverService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverFunctions"/> class.
        /// </summary>
        /// <param name="driverService">The driver service.</param>
        public DriverFunctions(IDriverService driverService)
        {
            this.driverService = driverService;
        }

        /// <summary>
        /// Gets all drivers.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>The HTTP response containing the drivers.</returns>
        [Function("GetDrivers")]
        public async Task<HttpResponseData> GetDrivers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "drivers")] HttpRequestData req)
        {
            var drivers = await this.driverService.GetDriversAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(drivers);
            return response;
        }

        /// <summary>
        /// Gets a driver by ID.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="id">The driver ID.</param>
        /// <returns>The HTTP response containing the driver.</returns>
        [Function("GetDriver")]
        public async Task<HttpResponseData> GetDriver(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "drivers/{id}")] HttpRequestData req,
            int id)
        {
            var driver = await this.driverService.GetDriverByIdAsync(id);
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

        /// <summary>
        /// Creates a new driver.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <returns>The HTTP response containing the created driver.</returns>
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

            var createdDriver = await this.driverService.CreateDriverAsync(driver);
            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(createdDriver);
            return response;
        }
    }
}
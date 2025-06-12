using System.Net;
using HotshotLogistics.Core.Models;
using HotshotLogistics.Core.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;

namespace HotshotLogistics.Api.Functions;

public class JobTypeFunctions
{
    private readonly IJobTypeRepository _repository;

    public JobTypeFunctions(IJobTypeRepository repository)
    {
        _repository = repository;
    }

    [Function("GetJobTypes")]
    public async Task<HttpResponseData> GetJobTypes(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobtypes")] HttpRequestData req)
    {
        var jobTypes = await _repository.GetAllAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(jobTypes);
        return response;
    }

    [Function("GetJobType")]
    public async Task<HttpResponseData> GetJobType(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobtypes/{id}")] HttpRequestData req,
        int id)
    {
        var jobType = await _repository.GetByIdAsync(id);
        if (jobType == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"Job type with ID {id} not found");
            return notFoundResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(jobType);
        return response;
    }

    [Function("CreateJobType")]
    public async Task<HttpResponseData> CreateJobType(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobtypes")] HttpRequestData req)
    {
        var jobType = await JsonSerializer.DeserializeAsync<JobType>(req.Body);
        if (jobType == null)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid job type data");
            return badRequestResponse;
        }

        await _repository.AddAsync(jobType);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(jobType);
        return response;
    }
}

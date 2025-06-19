using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using HotshotLogistics.Contracts.Services;
using HotshotLogistics.Contracts.Models; // For IJob and JobDto
using System.Threading.Tasks;
using System.IO; // For StreamReader

namespace HotshotLogistics.Api.Functions;

public class JobFunctions
{
    private readonly IJobService _jobService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };


    public JobFunctions(IJobService jobService)
    {
        _jobService = jobService;
    }

    [Function("GetJobs")]
    public async Task<HttpResponseData> GetJobs(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs")] HttpRequestData req)
    {
        var jobs = await _jobService.GetJobsAsync();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(jobs);
        return response;
    }

    [Function("GetJobById")]
    public async Task<HttpResponseData> GetJobById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "jobs/{id}")] HttpRequestData req,
        string id)
    {
        var job = await _jobService.GetByIdAsync(id);
        if (job == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"Job with ID {id} not found");
            return notFoundResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(job);
        return response;
    }

    [Function("CreateJob")]
    public async Task<HttpResponseData> CreateJob(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobs")] HttpRequestData req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var jobDto = JsonSerializer.Deserialize<JobDto>(requestBody, _jsonSerializerOptions);

        if (jobDto == null) // Basic validation
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid job data provided.");
            return badRequestResponse;
        }

        // Add more specific validation as needed (e.g., required fields)

        var createdJob = await _jobService.CreateJobAsync(jobDto);
        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(createdJob);
        return response;
    }

    [Function("UpdateJob")]
    public async Task<HttpResponseData> UpdateJob(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "jobs/{id}")] HttpRequestData req,
        string id)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var jobDto = JsonSerializer.Deserialize<JobDto>(requestBody, _jsonSerializerOptions);

        if (jobDto == null)
        {
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Invalid job data provided.");
            return badRequestResponse;
        }

        var updatedJob = await _jobService.UpdateJobAsync(id, jobDto);
        if (updatedJob == null)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"Job with ID {id} not found for update.");
            return notFoundResponse;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(updatedJob);
        return response;
    }

    [Function("DeleteJob")]
    public async Task<HttpResponseData> DeleteJob(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "jobs/{id}")] HttpRequestData req,
        string id)
    {
        var success = await _jobService.DeleteJobAsync(id);
        if (!success)
        {
            var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await notFoundResponse.WriteStringAsync($"Job with ID {id} not found or could not be deleted.");
            return notFoundResponse;
        }

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
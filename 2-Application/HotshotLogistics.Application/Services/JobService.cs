using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Contracts.Repositories;
using HotshotLogistics.Contracts.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotshotLogistics.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public Task<IJob> CreateJobAsync(IJob job)
    {
        // Add any business logic, validation, etc., here before calling the repository
        return _jobRepository.CreateJobAsync(job);
    }

    public Task<IJob?> GetJobByIdAsync(string id)
    {
        return _jobRepository.GetJobByIdAsync(id);
    }

    public Task<IEnumerable<IJob>> GetJobsAsync()
    {
        return _jobRepository.GetJobsAsync();
    }

    public Task<IJob?> UpdateJobAsync(string id, IJob jobDetails)
    {
        // Add any business logic, validation, etc.
        return _jobRepository.UpdateJobAsync(id, jobDetails);
    }

    public Task<bool> DeleteJobAsync(string id)
    {
        // Add any business logic (e.g., check if job can be deleted)
        return _jobRepository.DeleteJobAsync(id);
    }
}
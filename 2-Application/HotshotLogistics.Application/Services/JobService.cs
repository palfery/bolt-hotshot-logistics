using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Contracts.Repositories;
using HotshotLogistics.Contracts.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HotshotLogistics.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public Task<IJob> CreateJobAsync(IJob job, CancellationToken cancellationToken = default)
    {
        // Add any business logic, validation, etc., here before calling the repository
        return _jobRepository.CreateJobAsync(job, cancellationToken);
    }

    public Task<IJob?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _jobRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<IJob>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        return _jobRepository.GetJobsAsync(cancellationToken);
    }

    public Task<IJob?> UpdateJobAsync(string id, IJob jobDetails, CancellationToken cancellationToken = default)
    {
        // Add any business logic, validation, etc.
        return _jobRepository.UpdateJobAsync(id, jobDetails, cancellationToken);
    }

    public Task<bool> DeleteJobAsync(string id, CancellationToken cancellationToken = default)
    {
        // Add any business logic (e.g., check if job can be deleted)
        return _jobRepository.DeleteJobAsync(id, cancellationToken);
    }
}
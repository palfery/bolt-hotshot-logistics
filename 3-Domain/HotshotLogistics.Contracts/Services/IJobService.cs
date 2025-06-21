using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Contracts.Services;

public interface IJobService
{
    Task<IEnumerable<IJob>> GetJobsAsync(CancellationToken cancellationToken = default);
    Task<IJob?> GetJobByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IJob> CreateJobAsync(IJob job, CancellationToken cancellationToken = default);
    Task<IJob?> UpdateJobAsync(string id, IJob jobDetails, CancellationToken cancellationToken = default);
    Task<bool> DeleteJobAsync(string id, CancellationToken cancellationToken = default);
}

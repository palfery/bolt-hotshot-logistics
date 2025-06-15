using HotshotLogistics.Contracts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotshotLogistics.Contracts.Repositories;

public interface IJobRepository
{
    Task<IEnumerable<IJob>> GetJobsAsync();
    Task<IJob?> GetJobByIdAsync(string id);
    Task<IJob> CreateJobAsync(IJob job);
    Task<IJob?> UpdateJobAsync(string id, IJob jobDetails);
    Task<bool> DeleteJobAsync(string id);
    // Add other specific query methods if needed, e.g., GetJobsByStatusAsync
}
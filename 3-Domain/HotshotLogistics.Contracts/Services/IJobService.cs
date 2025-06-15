using HotshotLogistics.Contracts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotshotLogistics.Contracts.Services;

public interface IJobService
{
    Task<IEnumerable<IJob>> GetJobsAsync();
    Task<IJob?> GetJobByIdAsync(string id);
    Task<IJob> CreateJobAsync(IJob job); // Accepts IJob (DTO)
    Task<IJob?> UpdateJobAsync(string id, IJob jobDetails); // Accepts IJob (DTO)
    Task<bool> DeleteJobAsync(string id);
}
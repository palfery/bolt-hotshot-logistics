namespace HotshotLogistics.Core.Interfaces;

using HotshotLogistics.Core.Models;

public interface IJobTypeRepository
{
    Task<List<JobType>> GetAllAsync();
    Task<JobType?> GetByIdAsync(int id);
    Task<JobType> AddAsync(JobType jobType);
}

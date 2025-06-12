using HotshotLogistics.Core.Interfaces;
using HotshotLogistics.Core.Models;
using HotshotLogistics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotshotLogistics.Infrastructure.Repositories;

public class JobTypeRepository : IJobTypeRepository
{
    private readonly HotshotDbContext _context;

    public JobTypeRepository(HotshotDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobType>> GetAllAsync()
    {
        return await _context.JobTypes.ToListAsync();
    }

    public async Task<JobType?> GetByIdAsync(int id)
    {
        return await _context.JobTypes.FindAsync(id);
    }

    public async Task<JobType> AddAsync(JobType jobType)
    {
        var entry = await _context.JobTypes.AddAsync(jobType);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }
}

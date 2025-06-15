using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Contracts.Repositories;
using HotshotLogistics.Domain.Models; // Actual domain model
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotshotLogistics.Data.Repositories;

public class JobRepository : IJobRepository
{
    private readonly HotshotDbContext _context;

    public JobRepository(HotshotDbContext context)
    {
        _context = context;
    }

    public async Task<IJob> CreateJobAsync(IJob jobDto)
    {
        var job = new Job // Map from IJob DTO to Domain.Model.Job
        {
            Id = string.IsNullOrEmpty(jobDto.Id) ? Guid.NewGuid().ToString() : jobDto.Id,
            Title = jobDto.Title,
            PickupAddress = jobDto.PickupAddress,
            DropoffAddress = jobDto.DropoffAddress,
            Status = jobDto.Status,
            Priority = jobDto.Priority,
            Amount = jobDto.Amount,
            EstimatedDeliveryTime = jobDto.EstimatedDeliveryTime,
            AssignedDriverId = jobDto.AssignedDriverId,
            CreatedAt = DateTime.UtcNow // Ensure CreatedAt is set
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        // Map back to IJob or return the domain object if the service layer handles mapping
        // For simplicity, returning the domain object cast to IJob (if compatible, or map explicitly)
        // It's often better to have explicit mapping if IJob and Job diverge significantly.
        // Here, we assume Job implements IJob or has compatible properties.
        // Let's create a new DTO to return to ensure we return IJob.
        return MapToIJob(job);
    }

    public async Task<IJob?> GetJobByIdAsync(string id)
    {
        var job = await _context.Jobs
                                .AsNoTracking()
                                .FirstOrDefaultAsync(j => j.Id == id);
        return job != null ? MapToIJob(job) : null;
    }

    public async Task<IEnumerable<IJob>> GetJobsAsync()
    {
        var jobs = await _context.Jobs
                                 .AsNoTracking()
                                 .ToListAsync();
        return jobs.Select(MapToIJob);
    }

    public async Task<IJob?> UpdateJobAsync(string id, IJob jobDetails)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
        {
            return null;
        }

        job.Title = jobDetails.Title;
        job.PickupAddress = jobDetails.PickupAddress;
        job.DropoffAddress = jobDetails.DropoffAddress;
        job.Status = jobDetails.Status;
        job.Priority = jobDetails.Priority;
        job.Amount = jobDetails.Amount;
        job.EstimatedDeliveryTime = jobDetails.EstimatedDeliveryTime;
        job.AssignedDriverId = jobDetails.AssignedDriverId;
        job.UpdatedAt = DateTime.UtcNow;

        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
        return MapToIJob(job);
    }

    public async Task<bool> DeleteJobAsync(string id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
        {
            return false;
        }

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }

    // Helper for mapping. Consider using a library like AutoMapper for complex scenarios.
    private static IJob MapToIJob(Job job)
    {
        // This is a simple manual mapping.
        // If IJob is an interface that Job implements, you can cast.
        // Otherwise, you need to create a DTO instance.
        // For this example, let's assume we need to create a DTO that implements IJob.
        // If you create a concrete DTO class (e.g., JobDto : IJob), instantiate that.
        // For now, we'll use the domain model directly if it's compatible or map to a new object.
        // Let's create a simple DTO class for this purpose or use an anonymous type if IJob allows.
        // To keep it clean, let's assume Job itself can be returned if IJob is simple enough.
        // However, the request implies IJob is a contract, so we should map.
        // We'll need a concrete class that implements IJob for the return types.
        // Let's define a simple one here for demonstration or assume one exists.

        // Option 1: If Job implements IJob (add ' : IJob' to Job class definition)
        // return job;

        // Option 2: Manual mapping to a new object (if IJob is just an interface)
        // This requires a concrete class that implements IJob.
        // Let's define a simple concrete DTO for this.
        return new HotshotLogistics.Contracts.Models.JobDto // Assuming JobDto : IJob exists
        {
            Id = job.Id,
            Title = job.Title,
            PickupAddress = job.PickupAddress,
            DropoffAddress = job.DropoffAddress,
            Status = job.Status,
            Priority = job.Priority,
            Amount = job.Amount,
            EstimatedDeliveryTime = job.EstimatedDeliveryTime,
            AssignedDriverId = job.AssignedDriverId,
            CreatedAt = job.CreatedAt,
            UpdatedAt = job.UpdatedAt
        };
    }
}
using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Contracts.Repositories;
using HotshotLogistics.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HotshotLogistics.Data.Repositories;

/// <summary>
/// Repository implementation for JobAssignment operations.
/// </summary>
internal class JobAssignmentRepository : IJobAssignmentRepository
{
    private readonly HotshotDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobAssignmentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public JobAssignmentRepository(HotshotDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<JobAssignmentDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.JobAssignments
            .Include(a => a.Driver)
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return MapToDto(assignment);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<JobAssignmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var assignments = await _context.JobAssignments
            .Include(a => a.Driver)
            .Include(a => a.Job)
            .ToListAsync(cancellationToken);

        return assignments.Select(MapToDto).Where(dto => dto != null)!;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<JobAssignmentDto>> GetByDriverIdAsync(int driverId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.JobAssignments
            .Include(a => a.Driver)
            .Include(a => a.Job)
            .Where(a => a.DriverId == driverId)
            .ToListAsync(cancellationToken);

        return assignments.Select(MapToDto).Where(dto => dto != null)!;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<JobAssignmentDto>> GetByJobIdAsync(string jobId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.JobAssignments
            .Include(a => a.Driver)
            .Include(a => a.Job)
            .Where(a => a.JobId == jobId)
            .ToListAsync(cancellationToken);

        return assignments.Select(MapToDto).Where(dto => dto != null)!;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<JobAssignmentDto>> GetActiveAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var assignments = await _context.JobAssignments
            .Include(a => a.Driver)
            .Include(a => a.Job)
            .Where(a => a.Status == JobAssignmentStatus.Active)
            .ToListAsync(cancellationToken);

        return assignments.Select(MapToDto).Where(dto => dto != null)!;
    }

    /// <inheritdoc/>
    public async Task<JobAssignmentDto> CreateAsync(JobAssignmentDto jobAssignment, CancellationToken cancellationToken = default)
    {
        if (jobAssignment == null)
        {
            throw new ArgumentNullException(nameof(jobAssignment));
        }

        var entity = new JobAssignment
        {
            JobId = jobAssignment.JobId,
            DriverId = jobAssignment.DriverId,
            AssignedAt = jobAssignment.AssignedAt,
            Status = jobAssignment.Status,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.JobAssignments.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return assignments.Select(MapToDto).Where(dto => dto != null);
        return (await GetByIdAsync(entity.Id, cancellationToken))!;
    }

    /// <inheritdoc/>
    public async Task<JobAssignmentDto> UpdateAsync(JobAssignmentDto jobAssignment, CancellationToken cancellationToken = default)
    {
        if (jobAssignment == null)
        {
            throw new ArgumentNullException(nameof(jobAssignment));
        }

        var existing = await _context.JobAssignments
            .FirstOrDefaultAsync(a => a.Id == jobAssignment.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Job assignment with ID {jobAssignment.Id} not found.");

        existing.JobId = jobAssignment.JobId;
        existing.DriverId = jobAssignment.DriverId;
        existing.AssignedAt = jobAssignment.AssignedAt;
        existing.Status = jobAssignment.Status;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Reload to get navigation properties
        return (await GetByIdAsync(existing.Id, cancellationToken))!;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.JobAssignments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (assignment == null)
        {
            return false;
        }

        _context.JobAssignments.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static JobAssignmentDto? MapToDto(JobAssignment? assignment)
    {
        if (assignment == null)
        {
            return null;
        }

        return new JobAssignmentDto
        {
            Id = assignment.Id,
            JobId = assignment.JobId,
            DriverId = assignment.DriverId,
            AssignedAt = assignment.AssignedAt,
            Status = assignment.Status,
            Driver = assignment.Driver != null
                ? new DriverDto
                {
                    Id = assignment.Driver.Id,
                    FirstName = assignment.Driver.FirstName,
                    LastName = assignment.Driver.LastName,
                    Email = assignment.Driver.Email,
                    PhoneNumber = assignment.Driver.PhoneNumber,
                    LicenseNumber = assignment.Driver.LicenseNumber,
                    LicenseExpiryDate = assignment.Driver.LicenseExpiryDate,
                }
                : null,
            Job = assignment.Job != null
                ? new JobDto
                {
                    Id = assignment.Job.Id,
                    Title = assignment.Job.Title,
                    PickupAddress = assignment.Job.PickupAddress,
                    DropoffAddress = assignment.Job.DropoffAddress,
                    Status = assignment.Job.Status,
                    Priority = assignment.Job.Priority,
                    Amount = assignment.Job.Amount,
                    EstimatedDeliveryTime = assignment.Job.EstimatedDeliveryTime,
                    CreatedAt = assignment.Job.CreatedAt,
                    UpdatedAt = assignment.Job.UpdatedAt,
                }
                : null,
        };
    }
}

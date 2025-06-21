// <copyright file="JobRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Contracts.Repositories;
    using HotshotLogistics.Domain.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for managing Job entities.
    /// </summary>
    internal class JobRepository : IJobRepository
    {
        private readonly HotshotDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public JobRepository(HotshotDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await this.dbContext.Jobs
                .Include(j => j.AssignedDriver)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Job?> GetByIdAsync(string id)
        {
            return await this.dbContext.Jobs
                .Include(j => j.AssignedDriver)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        /// <inheritdoc/>
        public async Task<Job> AddAsync(Job job)
        {
            await this.dbContext.Jobs.AddAsync(job);
            await this.dbContext.SaveChangesAsync();
            return job;
        }

        /// <inheritdoc/>
        public async Task<Job> UpdateAsync(Job job)
        {
            this.dbContext.Jobs.Update(job);
            await this.dbContext.SaveChangesAsync();
            return job;
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string id)
        {
            var job = await this.dbContext.Jobs.FindAsync(id);
            if (job != null)
            {
                this.dbContext.Jobs.Remove(job);
                await this.dbContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<IJob> CreateJobAsync(IJob jobDto, CancellationToken cancellationToken = default)
        {
            var job = new Job
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
                CreatedAt = DateTime.UtcNow,
            };

            this.dbContext.Jobs.Add(job);
            await this.dbContext.SaveChangesAsync(cancellationToken);
            // Map to JobDto
            return new JobDto
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
                UpdatedAt = job.UpdatedAt,
            };
        }

        /// <inheritdoc/>
        public async Task<IJob?> GetJobByIdAsync(string id)
        {
            var job = await this.dbContext.Jobs
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(j => j.Id == id);
            return job;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            var jobs = await this.dbContext.Jobs
                                      .AsNoTracking()
                                      .ToListAsync(cancellationToken);
            return jobs;
        }

        /// <inheritdoc/>
        public async Task<IJob?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var job = await this.dbContext.Jobs
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
            return job;
        }

        /// <inheritdoc/>
        public async Task<IJob?> UpdateJobAsync(string id, IJob jobDetails, CancellationToken cancellationToken = default)
        {
            var job = await this.dbContext.Jobs.FindAsync(new object[] { id }, cancellationToken);
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

            this.dbContext.Jobs.Update(job);
            await this.dbContext.SaveChangesAsync(cancellationToken);
            return job;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteJobAsync(string id, CancellationToken cancellationToken = default)
        {
            var job = await this.dbContext.Jobs.FindAsync(new object[] { id }, cancellationToken);
            if (job == null)
            {
                return false;
            }

            this.dbContext.Jobs.Remove(job);
            await this.dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

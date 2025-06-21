// <copyright file="JobService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Application.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Contracts.Repositories;
    using HotshotLogistics.Contracts.Services;

    /// <summary>
    /// Service for managing jobs.
    /// </summary>
    public class JobService : IJobService
    {
        private readonly IJobRepository jobRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobService"/> class.
        /// </summary>
        /// <param name="jobRepository">The job repository.</param>
        public JobService(IJobRepository jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        /// <inheritdoc/>
        public Task<IJob> CreateJobAsync(IJob job, CancellationToken cancellationToken = default)
        {
            // Add any business logic, validation, etc., here before calling the repository

            return this.jobRepository.CreateJobAsync(job, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IJob?> GetJobByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return this.jobRepository.GetJobByIdAsync(id, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            return this.jobRepository.GetJobsAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IJob?> UpdateJobAsync(string id, IJob jobDetails, CancellationToken cancellationToken = default)
        {
            // Add any business logic, validation, etc.

            return this.jobRepository.UpdateJobAsync(id, jobDetails, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteJobAsync(string id, CancellationToken cancellationToken = default)
        {
            // Add any business logic (e.g., check if job can be deleted)
            return this.jobRepository.DeleteJobAsync(id, cancellationToken);
        }
    }
}

// <copyright file="Job.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Domain.Models
{
    using System;
    using HotshotLogistics.Contracts.Models;

    /// <summary>
    /// Represents a job in the logistics system.
    /// </summary>
    public class Job : IJob
    {
        /// <summary>
        /// Gets or sets the unique identifier for the job.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Primary Key

        /// <summary>
        /// Gets or sets the title of the job.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the pickup address for the job.
        /// </summary>
        public string PickupAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the dropoff address for the job.
        /// </summary>
        public string DropoffAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current status of the job.
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the priority level of the job.
        /// </summary>
        public JobPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the monetary amount for the job.
        /// </summary>
        public decimal Amount { get; set; } // Using decimal for currency

        /// <summary>
        /// Gets or sets the estimated delivery time for the job.
        /// </summary>
        public string EstimatedDeliveryTime { get; set; } = string.Empty; // Or consider TimeSpan/DateTimeOffset

        /// <summary>
        /// Gets or sets the ID of the assigned driver.
        /// </summary>
        public int? AssignedDriverId { get; set; } // Foreign Key to Driver

        /// <summary>
        /// Gets or sets the assigned driver for this job.
        /// </summary>
        public Driver? AssignedDriver { get; set; } // Navigation property

        /// <summary>
        /// Gets or sets the creation timestamp of the job.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update timestamp of the job.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Job"/> class.
        /// </summary>
        public Job()
        {
            if (CreatedAt == default)
                CreatedAt = DateTime.UtcNow;
        }
    }
}

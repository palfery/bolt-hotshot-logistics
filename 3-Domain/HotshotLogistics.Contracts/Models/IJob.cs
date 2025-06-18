// <copyright file="IJob.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Contracts.Models
{
    using System;

    /// <summary>
    /// Represents a job in the logistics system.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Gets or sets the unique identifier for the job.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the job.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the pickup address for the job.
        /// </summary>
        string PickupAddress { get; set; }

        /// <summary>
        /// Gets or sets the dropoff address for the job.
        /// </summary>
        string DropoffAddress { get; set; }

        /// <summary>
        /// Gets or sets the current status of the job.
        /// </summary>
        JobStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the priority level of the job.
        /// </summary>
        JobPriority Priority { get; set; }

        /// <summary>
        /// Gets or sets the monetary amount for the job.
        /// </summary>
        decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the estimated delivery time for the job.
        /// </summary>
        string EstimatedDeliveryTime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the assigned driver.
        /// </summary>
        int? AssignedDriverId { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp of the job.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update timestamp of the job.
        /// </summary>
        DateTime? UpdatedAt { get; set; }
    }
}
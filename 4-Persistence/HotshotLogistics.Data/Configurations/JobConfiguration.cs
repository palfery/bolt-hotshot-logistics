// <copyright file="JobConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data.Configurations
{
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;

    /// <summary>
    /// Configuration for the Job entity.
    /// </summary>
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.PickupAddress)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(j => j.DropoffAddress)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(j => j.Status)
                .IsRequired()
                .HasConversion(
                    s => s.ToString(),
                    s => (HotshotLogistics.Contracts.Models.JobStatus)Enum.Parse(typeof(HotshotLogistics.Contracts.Models.JobStatus), s))
                .HasMaxLength(50);

            builder.Property(j => j.Priority)
                .IsRequired()
                .HasConversion(
                    p => p.ToString(),
                    p => (HotshotLogistics.Contracts.Models.JobPriority)Enum.Parse(typeof(HotshotLogistics.Contracts.Models.JobPriority), p))
                .HasMaxLength(50);

            builder.Property(j => j.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(j => j.EstimatedDeliveryTime)
                .IsRequired();

            builder.Property(j => j.AssignedDriverId)
                .IsRequired(false);

            builder.Property(j => j.CreatedAt)
                .IsRequired();

            builder.Property(j => j.UpdatedAt)
                .IsRequired(false);

            builder.HasOne(j => j.AssignedDriver)
                .WithMany()
                .HasForeignKey(j => j.AssignedDriverId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(
                new Job
                {
                    Id = "job-1",
                    Title = "Deliver Package A",
                    PickupAddress = "123 Main St",
                    DropoffAddress = "456 Elm St",
                    Status = JobStatus.Pending,
                    Priority = JobPriority.High,
                    Amount = 100.00m,
                    EstimatedDeliveryTime = "2024-06-16T10:00:00Z",
                    AssignedDriverId = 1,
                    CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                },
                new Job
                {
                    Id = "job-2",
                    Title = "Deliver Package B",
                    PickupAddress = "789 Oak St",
                    DropoffAddress = "321 Pine St",
                    Status = JobStatus.InTransit,
                    Priority = JobPriority.Medium,
                    Amount = 75.50m,
                    EstimatedDeliveryTime = "2024-06-17T14:00:00Z",
                    AssignedDriverId = 2,
                    CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                });
        }
    }
}

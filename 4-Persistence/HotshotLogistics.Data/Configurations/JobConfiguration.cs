using HotshotLogistics.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotshotLogistics.Domain.Enums;

namespace HotshotLogistics.Data.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
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
                s => (JobStatus)Enum.Parse(typeof(JobStatus), s))
            .HasMaxLength(50);

        builder.Property(j => j.Priority)
            .IsRequired()
            .HasConversion(
                p => p.ToString(),
                p => (JobPriority)Enum.Parse(typeof(JobPriority), p))
            .HasMaxLength(50);

        builder.Property(j => j.Amount)
            .HasColumnType("decimal(18,2)"); // Specify precision for currency

        builder.Property(j => j.EstimatedDeliveryTime)
            .HasMaxLength(100); // Adjust as needed

        builder.HasOne(j => j.AssignedDriver)
            .WithMany() // Assuming a Driver can have many Jobs, but a Job has one Driver.
                       // If a Driver has a collection of Jobs, define it in Driver model and use WithMany(d => d.Jobs)
            .HasForeignKey(j => j.AssignedDriverId)
            .OnDelete(DeleteBehavior.SetNull); // Or Restrict, depending on business rules

        builder.Property(j => j.CreatedAt);
        builder.Property(j => j.UpdatedAt);

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
                CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc)
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
                CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
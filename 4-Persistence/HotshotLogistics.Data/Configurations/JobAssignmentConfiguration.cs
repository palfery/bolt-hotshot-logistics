using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotshotLogistics.Domain.Models;

namespace HotshotLogistics.Data.Configurations;

/// <summary>
/// Configuration for the JobAssignment entity.
/// </summary>
public class JobAssignmentConfiguration : IEntityTypeConfiguration<JobAssignment>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<JobAssignment> builder)
    {
        builder.ToTable("JobAssignments");

        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .IsRequired()
            .HasMaxLength(36); // Length of GUID as string
            
        builder.Property(a => a.JobId)
            .IsRequired()
            .HasMaxLength(36); // Length of GUID as string
            
        builder.Property(a => a.DriverId)
            .IsRequired();
            
        builder.Property(a => a.AssignedAt)
            .IsRequired()
            .HasColumnType("datetime2");
            
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(a => a.UpdatedAt)
            .HasColumnType("datetime2");
            
        // Relationships
        builder.HasOne(a => a.Job)
            .WithMany()
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(a => a.Driver)
            .WithMany()
            .HasForeignKey(a => a.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(a => a.JobId);
        builder.HasIndex(a => a.DriverId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.AssignedAt);
    }
}

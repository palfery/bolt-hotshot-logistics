using HotshotLogistics.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotshotLogistics.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(e => e.LicenseNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
        builder.Property(e => e.UpdatedAt)
            .ValueGeneratedOnUpdate();
    }
} 
using HotshotLogistics.Domain.Models;
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
            
        builder.Property(e => e.LicenseExpiryDate);
        builder.Property(e => e.IsActive);
        builder.Property(e => e.CreatedAt);
        builder.Property(e => e.UpdatedAt);

        builder.HasData(
            new Driver
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com",
                PhoneNumber = "555-1234",
                LicenseNumber = "A1234567",
                LicenseExpiryDate = new DateTime(2030, 1, 1),
                IsActive = true,
                CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Driver
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                PhoneNumber = "555-5678",
                LicenseNumber = "B7654321",
                LicenseExpiryDate = new DateTime(2031, 12, 31),
                IsActive = true,
                CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
} 
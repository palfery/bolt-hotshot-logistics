// <copyright file="DriverConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data.Configurations
{
    using HotshotLogistics.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Configuration for the Driver entity.
    /// </summary>
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(d => d.LicenseNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(d => d.LicenseExpiryDate)
                .IsRequired();

            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.UpdatedAt)
                .IsRequired(false);

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
                    CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
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
                    CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                });
        }
    }
}
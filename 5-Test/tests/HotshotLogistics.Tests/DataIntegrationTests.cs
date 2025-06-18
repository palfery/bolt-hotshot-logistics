// <copyright file="DataIntegrationTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Data;
    using HotshotLogistics.Data.Repositories;
    using HotshotLogistics.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    /// <summary>
    /// Integration tests for the data layer.
    /// </summary>
    public class DataIntegrationTests
    {
        /// <summary>
        /// Creates a new in-memory database context for testing.
        /// </summary>
        /// <returns>A new database context.</returns>
        private HotshotDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<HotshotDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new HotshotDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        /// <summary>
        /// Tests that seeded drivers exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task SeededDriversExist()
        {
            using var context = CreateContext();
            var drivers = await context.Drivers.ToListAsync();
            Assert.True(drivers.Count >= 2);
            Assert.Contains(drivers, d => d.FirstName == "Alice" && d.LastName == "Smith");
            Assert.Contains(drivers, d => d.FirstName == "Bob" && d.LastName == "Johnson");
        }

        /// <summary>
        /// Tests that a driver can be created and retrieved.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanCreateAndRetrieveDriver()
        {
            using var context = CreateContext();
            var repo = new DriverRepository(context);
            var newDriver = new Driver
            {
                FirstName = "Test",
                LastName = "Driver",
                Email = "test.driver@example.com",
                PhoneNumber = "555-0000",
                LicenseNumber = "T1234567",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(5),
                IsActive = true
            };
            var created = await repo.CreateDriverAsync(newDriver);
            Assert.NotEqual(0, created.Id);
            var fetched = await repo.GetDriverByIdAsync(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal("Test", fetched!.FirstName);
        }

        /// <summary>
        /// Tests that a driver can be updated.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanUpdateDriver()
        {
            using var context = CreateContext();
            var repo = new DriverRepository(context);
            var driver = context.Drivers.First();
            driver.LastName = "Updated";
            await repo.UpdateDriverAsync(driver);
            var updated = await repo.GetDriverByIdAsync(driver.Id);
            Assert.Equal("Updated", updated!.LastName);
        }

        /// <summary>
        /// Tests that a driver can be deleted.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanDeleteDriver()
        {
            using var context = CreateContext();
            var repo = new DriverRepository(context);
            var driver = context.Drivers.OrderByDescending(d => d.Id).First();
            await repo.DeleteDriverAsync(driver.Id);
            var deleted = await repo.GetDriverByIdAsync(driver.Id);
            Assert.Null(deleted);
        }

        /// <summary>
        /// Tests that seeded jobs exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task SeededJobsExist()
        {
            using var context = CreateContext();
            var jobs = await context.Jobs.ToListAsync();
            Assert.True(jobs.Count >= 2);
            Assert.Contains(jobs, j => j.Title == "Deliver Package A");
            Assert.Contains(jobs, j => j.Title == "Deliver Package B");
        }

        /// <summary>
        /// Tests that a job can be created and retrieved.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanCreateAndRetrieveJob()
        {
            using var context = CreateContext();
            var repo = new JobRepository(context);
            var newJob = new Job
            {
                Title = "Integration Test Job",
                PickupAddress = "Test Pickup",
                DropoffAddress = "Test Dropoff",
                Status = JobStatus.Pending,
                Priority = JobPriority.Low,
                Amount = 10.0m,
                EstimatedDeliveryTime = "2024-07-01T10:00:00Z"
            };
            var jobDto = new JobDto
            {
                Id = newJob.Id,
                Title = newJob.Title,
                PickupAddress = newJob.PickupAddress,
                DropoffAddress = newJob.DropoffAddress,
                Status = newJob.Status,
                Priority = newJob.Priority,
                Amount = newJob.Amount,
                EstimatedDeliveryTime = newJob.EstimatedDeliveryTime,
                AssignedDriverId = newJob.AssignedDriverId,
                CreatedAt = newJob.CreatedAt,
                UpdatedAt = newJob.UpdatedAt
            };
            var created = await repo.CreateJobAsync(jobDto);
            Assert.NotNull(created.Id);
            var fetched = await context.Jobs.FindAsync(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal("Integration Test Job", fetched!.Title);
        }

        /// <summary>
        /// Tests that a job can be updated.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanUpdateJob()
        {
            using var context = CreateContext();
            var repo = new JobRepository(context);
            var job = context.Jobs.First();
            job.Title = "Updated Title";
            var jobDto = new JobDto
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
                UpdatedAt = job.UpdatedAt
            };
            await repo.UpdateJobAsync(job.Id, jobDto);
            var updated = await context.Jobs.FindAsync(job.Id);
            Assert.Equal("Updated Title", updated!.Title);
        }

        /// <summary>
        /// Tests that a job can be deleted.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanDeleteJob()
        {
            using var context = CreateContext();
            var repo = new JobRepository(context);
            var job = context.Jobs.OrderByDescending(j => j.Id).First();
            context.Jobs.Remove(job);
            await context.SaveChangesAsync();
            var deleted = await context.Jobs.FindAsync(job.Id);
            Assert.Null(deleted);
        }

        /// <summary>
        /// Tests that a job's status can be transitioned.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanTransitionJobStatus()
        {
            using var context = CreateContext();
            var repo = new JobRepository(context);
            var job = context.Jobs.First();

            // Test status transitions
            job.Status = JobStatus.Assigned;
            var jobDto = new JobDto
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
                UpdatedAt = job.UpdatedAt
            };
            await repo.UpdateJobAsync(job.Id, jobDto);
            var updated = await context.Jobs.FindAsync(job.Id);
            Assert.Equal(JobStatus.Assigned, updated!.Status);

            job.Status = JobStatus.InTransit;
            jobDto.Status = job.Status;
            await repo.UpdateJobAsync(job.Id, jobDto);
            updated = await context.Jobs.FindAsync(job.Id);
            Assert.Equal(JobStatus.InTransit, updated!.Status);

            job.Status = JobStatus.Delivered;
            jobDto.Status = job.Status;
            await repo.UpdateJobAsync(job.Id, jobDto);
            updated = await context.Jobs.FindAsync(job.Id);
            Assert.Equal(JobStatus.Delivered, updated!.Status);
        }

        /// <summary>
        /// Tests that a job's priority can be changed.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanChangeJobPriority()
        {
            using var context = CreateContext();
            var repo = new JobRepository(context);
            var job = context.Jobs.First();
            var jobDto = new JobDto
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
                UpdatedAt = job.UpdatedAt
            };

            // Test priority changes
            job.Priority = JobPriority.Low;
            jobDto.Priority = job.Priority;
            await repo.UpdateJobAsync(job.Id, jobDto);
            var updated = await context.Jobs.FindAsync(job.Id);
            Assert.Equal(JobPriority.Low, updated!.Priority);

            job.Priority = JobPriority.Medium;
            jobDto.Priority = job.Priority;
            await repo.UpdateJobAsync(job.Id, jobDto);
            updated = await context.Jobs.FindAsync(job.Id);
            Assert.Equal(JobPriority.Medium, updated!.Priority);

            job.Priority = JobPriority.High;
            jobDto.Priority = job.Priority;
            await repo.UpdateJobAsync(job.Id, jobDto);
            updated = await context.Jobs.FindAsync(job.Id);
            Assert.Equal(JobPriority.High, updated!.Priority);
        }

        /// <summary>
        /// Tests that a driver can be assigned to a job.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanAssignDriverToJob()
        {
            using var context = CreateContext();
            var repo = new JobRepository(context);
            var job = context.Jobs.First();
            var driver = context.Drivers.First();

            // Assign driver to job
            job.AssignedDriverId = driver.Id;
            var jobDto = new JobDto
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
                UpdatedAt = job.UpdatedAt
            };
            await repo.UpdateJobAsync(job.Id, jobDto);

            // Verify assignment
            var updated = await context.Jobs
                .Include(j => j.AssignedDriver)
                .FirstOrDefaultAsync(j => j.Id == job.Id);
            Assert.NotNull(updated);
            Assert.Equal(driver.Id, updated!.AssignedDriverId);
            Assert.NotNull(updated.AssignedDriver);
            Assert.Equal(driver.FirstName, updated.AssignedDriver!.FirstName);
        }

        /// <summary>
        /// Tests that a driver's status can be updated.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanUpdateDriverStatus()
        {
            using var context = CreateContext();
            var repo = new DriverRepository(context);
            var driver = context.Drivers.First();

            // Test deactivating driver
            driver.IsActive = false;
            await repo.UpdateDriverAsync(driver);
            var updated = await repo.GetDriverByIdAsync(driver.Id);
            Assert.False(updated!.IsActive);

            // Test reactivating driver
            driver.IsActive = true;
            await repo.UpdateDriverAsync(driver);
            updated = await repo.GetDriverByIdAsync(driver.Id);
            Assert.True(updated!.IsActive);
        }

        /// <summary>
        /// Tests that a driver's license can be updated.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task CanUpdateDriverLicense()
        {
            using var context = CreateContext();
            var repo = new DriverRepository(context);
            var driver = context.Drivers.First();

            // Update license information
            var newExpiryDate = DateTime.UtcNow.AddYears(2);
            var newLicenseNumber = "NEW123456";
            driver.LicenseExpiryDate = newExpiryDate;
            driver.LicenseNumber = newLicenseNumber;
            await repo.UpdateDriverAsync(driver);

            // Verify updates
            var updated = await repo.GetDriverByIdAsync(driver.Id);
            Assert.NotNull(updated);
            Assert.Equal(newExpiryDate.Date, updated!.LicenseExpiryDate.Date);
            Assert.Equal(newLicenseNumber, updated.LicenseNumber);
        }
    }
}
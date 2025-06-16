using System;
using HotshotLogistics.Domain.Models;
using HotshotLogistics.Contracts.Models;
using Xunit;

namespace HotshotLogistics.Tests;

public class JobAssignmentTests
{
    [Fact]
    public void Default_Constructor_Sets_Defaults()
    {
        var assignment = new JobAssignment();
        Assert.False(string.IsNullOrWhiteSpace(assignment.Id));
        Assert.Equal(string.Empty, assignment.JobId);
        Assert.Equal(0, assignment.DriverId);
        Assert.True((DateTime.UtcNow - assignment.AssignedAt).TotalSeconds < 5); // AssignedAt is now
        Assert.Equal(JobAssignmentStatus.Active, assignment.Status);
        Assert.Null(assignment.UpdatedAt);
    }

    [Fact]
    public void Can_Assign_Properties()
    {
        var assignment = new JobAssignment
        {
            Id = "test-id",
            JobId = "job-123",
            DriverId = 42,
            AssignedAt = new DateTime(2024, 6, 16, 12, 0, 0, DateTimeKind.Utc),
            Status = JobAssignmentStatus.Completed,
            UpdatedAt = new DateTime(2024, 6, 17, 8, 0, 0, DateTimeKind.Utc)
        };
        Assert.Equal("test-id", assignment.Id);
        Assert.Equal("job-123", assignment.JobId);
        Assert.Equal(42, assignment.DriverId);
        Assert.Equal(new DateTime(2024, 6, 16, 12, 0, 0, DateTimeKind.Utc), assignment.AssignedAt);
        Assert.Equal(JobAssignmentStatus.Completed, assignment.Status);
        Assert.Equal(new DateTime(2024, 6, 17, 8, 0, 0, DateTimeKind.Utc), assignment.UpdatedAt);
    }

    [Fact]
    public void Can_Transition_Status()
    {
        var assignment = new JobAssignment();
        Assert.Equal(JobAssignmentStatus.Active, assignment.Status);
        assignment.Status = JobAssignmentStatus.Completed;
        Assert.Equal(JobAssignmentStatus.Completed, assignment.Status);
    }

    [Fact]
    public void Can_Set_Navigation_Properties()
    {
        var job = new Job { Id = "job-1", Title = "Test Job" };
        var driver = new Driver { Id = 1, FirstName = "Alice", LastName = "Smith" };
        var assignment = new JobAssignment
        {
            Job = job,
            Driver = driver
        };
        Assert.Equal(job, assignment.Job);
        Assert.Equal(driver, assignment.Driver);
    }
}
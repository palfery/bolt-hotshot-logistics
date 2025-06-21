namespace HotshotLogistics.Contracts.Models;

/// <summary>
/// Represents a job assignment to a driver.
/// </summary>
public interface IJobAssignment
{
    /// <summary>
    /// Gets or sets the assignment ID.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets or sets the related job ID.
    /// </summary>
    string JobId { get; set; }

    /// <summary>
    /// Gets or sets the assigned driver ID.
    /// </summary>
    int DriverId { get; set; }

    /// <summary>
    /// Gets or sets when the job was assigned.
    /// </summary>
    DateTime AssignedAt { get; set; }

    /// <summary>
    /// Gets or sets the assignment status.
    /// </summary>
    JobAssignmentStatus Status { get; set; }
}

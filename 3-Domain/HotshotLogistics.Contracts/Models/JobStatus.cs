namespace HotshotLogistics.Contracts.Models
{
    /// <summary>
    /// Represents the status of a job.
    /// </summary>
    public enum JobStatus
    {
        Pending,
        Assigned,
        InTransit,
        Delivered,
        Cancelled
    }
}

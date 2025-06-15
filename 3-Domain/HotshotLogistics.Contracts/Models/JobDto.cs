using System;

namespace HotshotLogistics.Contracts.Models;

public class JobDto : IJob
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public string DropoffAddress { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public JobPriority Priority { get; set; }
    public decimal Amount { get; set; }
    public string EstimatedDeliveryTime { get; set; } = string.Empty;
    public int? AssignedDriverId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
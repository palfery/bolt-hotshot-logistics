using System;
using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Domain.Models;

public class Job : IJob
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Primary Key
    public string Title { get; set; } = string.Empty;
    public string PickupAddress { get; set; } = string.Empty;
    public string DropoffAddress { get; set; } = string.Empty;
    public JobStatus Status { get; set; }
    public JobPriority Priority { get; set; }
    public decimal Amount { get; set; } // Using decimal for currency
    public string EstimatedDeliveryTime { get; set; } = string.Empty; // Or consider TimeSpan/DateTimeOffset

    public int? AssignedDriverId { get; set; } // Foreign Key to Driver
    public Driver? AssignedDriver { get; set; } // Navigation property

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Job()
    {
        if (CreatedAt == default)
            CreatedAt = DateTime.UtcNow;
    }
}
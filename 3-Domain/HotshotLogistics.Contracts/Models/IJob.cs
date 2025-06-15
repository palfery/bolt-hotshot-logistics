using System;

namespace HotshotLogistics.Contracts.Models;

public interface IJob
{
    string Id { get; set; }
    string Title { get; set; }
    string PickupAddress { get; set; }
    string DropoffAddress { get; set; }
    JobStatus Status { get; set; }
    JobPriority Priority { get; set; }
    decimal Amount { get; set; }
    string EstimatedDeliveryTime { get; set; }
    int? AssignedDriverId { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}
namespace HotshotLogistics.Domain.Models;

using HotshotLogistics.Contracts.Models;

/// <summary>
/// Represents a driver in the system.
/// </summary>
public class Driver : IDriver
{
    /// <inheritdoc/>
    public int Id { get; set; }

    /// <inheritdoc/>
    public string FirstName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string LastName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string Email { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string LicenseNumber { get; set; } = string.Empty;

    /// <inheritdoc/>
    public DateTime LicenseExpiryDate { get; set; }

    /// <inheritdoc/>
    public bool IsActive { get; set; }

    /// <inheritdoc/>
    public DateTime CreatedAt { get; set; }

    /// <inheritdoc/>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Driver"/> class.
    /// </summary>
    public Driver()
    {
        if (CreatedAt == default)
            CreatedAt = DateTime.UtcNow;
    }
}

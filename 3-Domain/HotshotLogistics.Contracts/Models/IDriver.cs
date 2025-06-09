namespace HotshotLogistics.Contracts.Models;

public interface IDriver
{
    int Id { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    string PhoneNumber { get; set; }
    string LicenseNumber { get; set; }
    DateTime LicenseExpiryDate { get; set; }
    bool IsActive { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

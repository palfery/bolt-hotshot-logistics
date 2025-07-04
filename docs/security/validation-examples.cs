// Input Validation Examples for Hotshot Logistics
// Add these validation attributes to your DTOs and request models

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace HotshotLogistics.Api.Models
{
    // Example: Enhanced AssignJobRequest with validation
    public class AssignJobRequest
    {
        [Required(ErrorMessage = "Job ID is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Job ID must be between 1 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9-_]+$", ErrorMessage = "Job ID can only contain alphanumeric characters, hyphens, and underscores")]
        public string JobId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Driver ID is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Driver ID must be between 1 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9-_]+$", ErrorMessage = "Driver ID can only contain alphanumeric characters, hyphens, and underscores")]
        public string DriverId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Assignment date is required")]
        [DataType(DataType.DateTime)]
        public DateTime AssignedDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [Range(1, 5, ErrorMessage = "Priority must be between 1 and 5")]
        public int Priority { get; set; } = 1;
    }

    // Example: Create Driver Request with comprehensive validation
    public class CreateDriverRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "First name can only contain letters, spaces, hyphens, and apostrophes")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Last name can only contain letters, spaces, hyphens, and apostrophes")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^\+?1?-?\.?\s?\(?([0-9]{3})\)?[-\.\s]?([0-9]{3})[-\.\s]?([0-9]{4})$", 
            ErrorMessage = "Phone number must be in valid US format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "License number is required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "License number must be between 5 and 20 characters")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "License number can only contain alphanumeric characters")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "License expiration date is required")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "License expiration date must be in the future")]
        public DateTime LicenseExpirationDate { get; set; }

        [Range(18, 75, ErrorMessage = "Driver age must be between 18 and 75")]
        public int Age { get; set; }

        [StringLength(1000, ErrorMessage = "Address cannot exceed 1000 characters")]
        public string? Address { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }

    // Example: Job creation with validation
    public class CreateJobRequest
    {
        [Required(ErrorMessage = "Job title is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Job title must be between 5 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pickup location is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Pickup location must be between 5 and 500 characters")]
        public string PickupLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Delivery location is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Delivery location must be between 5 and 500 characters")]
        public string DeliveryLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pickup date is required")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "Pickup date must be in the future")]
        public DateTime PickupDate { get; set; }

        [Required(ErrorMessage = "Delivery date is required")]
        [DataType(DataType.DateTime)]
        [DateGreaterThan("PickupDate", ErrorMessage = "Delivery date must be after pickup date")]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Payment amount is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Payment amount must be between $0.01 and $999,999.99")]
        [DataType(DataType.Currency)]
        public decimal PaymentAmount { get; set; }

        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters")]
        public string? CustomerName { get; set; }

        [StringLength(15, ErrorMessage = "Customer phone cannot exceed 15 characters")]
        [Phone(ErrorMessage = "Invalid customer phone format")]
        public string? CustomerPhone { get; set; }

        [StringLength(100, ErrorMessage = "Customer email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid customer email format")]
        public string? CustomerEmail { get; set; }
    }

    // Custom validation attributes
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }
            return false;
        }
    }

    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}

// Controller example with validation
namespace HotshotLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationExampleController : ControllerBase
    {
        private readonly ILogger<ValidationExampleController> _logger;

        public ValidationExampleController(ILogger<ValidationExampleController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> CreateDriver([FromBody] CreateDriverRequest request)
        {
            // Check model validation
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid driver creation request: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Additional custom validation
            if (await IsEmailAlreadyUsed(request.Email))
            {
                return BadRequest(new { message = "Email address is already in use" });
            }

            // Process valid request
            try
            {
                // Your business logic here
                return Ok(new { message = "Driver created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating driver");
                return StatusCode(500, new { message = "An error occurred while creating the driver" });
            }
        }

        private async Task<bool> IsEmailAlreadyUsed(string email)
        {
            // Implementation to check if email exists
            return false; // Placeholder
        }

        // Example of sanitizing input
        [HttpPost("search")]
        public ActionResult SearchDrivers([FromQuery] string searchTerm)
        {
            // Sanitize search term
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            // Remove potentially dangerous characters
            var sanitizedTerm = SanitizeSearchTerm(searchTerm);
            
            if (sanitizedTerm.Length < 2)
            {
                return BadRequest("Search term must be at least 2 characters after sanitization");
            }

            // Proceed with search
            return Ok(new { searchTerm = sanitizedTerm });
        }

        private string SanitizeSearchTerm(string input)
        {
            // Remove SQL injection characters and XSS attempts
            var dangerous = new[] { "'", "\"", "<", ">", "&", ";", "--", "/*", "*/" };
            var sanitized = input;

            foreach (var danger in dangerous)
            {
                sanitized = sanitized.Replace(danger, "");
            }

            // Trim and limit length
            return sanitized.Trim().Substring(0, Math.Min(sanitized.Length, 100));
        }
    }
}

// FluentValidation example (alternative to DataAnnotations)
/*
Install-Package FluentValidation.AspNetCore

using FluentValidation;

public class CreateDriverRequestValidator : AbstractValidator<CreateDriverRequest>
{
    public CreateDriverRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.LicenseExpirationDate)
            .GreaterThan(DateTime.Now).WithMessage("License expiration date must be in the future");

        RuleFor(x => x.Age)
            .InclusiveBetween(18, 75).WithMessage("Driver age must be between 18 and 75");
    }
}

// Register in Program.cs
services.AddFluentValidationAutoValidation();
services.AddFluentValidationClientsideAdapters();
services.AddValidatorsFromAssemblyContaining<CreateDriverRequestValidator>();
*/
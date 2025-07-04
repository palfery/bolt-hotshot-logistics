// <copyright file="DriverRequestValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;
using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Api.Validation
{
    /// <summary>
    /// Validator for driver creation and update requests.
    /// </summary>
    public class DriverRequestValidator : AbstractValidator<IDriver>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverRequestValidator"/> class.
        /// </summary>
        public DriverRequestValidator()
        {
            RuleFor(d => d.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(50)
                .WithMessage("First name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(d => d.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(50)
                .WithMessage("Last name cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z\s'-]+$")
                .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

            RuleFor(d => d.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(100)
                .WithMessage("Email cannot exceed 100 characters");

            RuleFor(d => d.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .Matches(@"^\+?[\d\s\-\(\)]+$")
                .WithMessage("Invalid phone number format")
                .MinimumLength(10)
                .WithMessage("Phone number must be at least 10 characters")
                .MaximumLength(20)
                .WithMessage("Phone number cannot exceed 20 characters");

            RuleFor(d => d.LicenseNumber)
                .NotEmpty()
                .WithMessage("License number is required")
                .MaximumLength(20)
                .WithMessage("License number cannot exceed 20 characters")
                .Matches(@"^[A-Z0-9]+$")
                .WithMessage("License number can only contain uppercase letters and numbers");

            RuleFor(d => d.LicenseExpiryDate)
                .NotEmpty()
                .WithMessage("License expiry date is required")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("License must not be expired");
        }
    }
}
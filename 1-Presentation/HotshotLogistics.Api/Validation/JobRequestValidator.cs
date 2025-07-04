// <copyright file="JobRequestValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using FluentValidation;
using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Api.Validation
{
    /// <summary>
    /// Validator for job creation and update requests.
    /// </summary>
    public class JobRequestValidator : AbstractValidator<IJob>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobRequestValidator"/> class.
        /// </summary>
        public JobRequestValidator()
        {
            RuleFor(j => j.Title)
                .NotEmpty()
                .WithMessage("Job title is required")
                .MaximumLength(200)
                .WithMessage("Job title cannot exceed 200 characters")
                .Matches(@"^[a-zA-Z0-9\s\-_.,!?()]+$")
                .WithMessage("Job title contains invalid characters");

            RuleFor(j => j.PickupAddress)
                .NotEmpty()
                .WithMessage("Pickup address is required")
                .MaximumLength(500)
                .WithMessage("Pickup address cannot exceed 500 characters");

            RuleFor(j => j.DropoffAddress)
                .NotEmpty()
                .WithMessage("Dropoff address is required")
                .MaximumLength(500)
                .WithMessage("Dropoff address cannot exceed 500 characters");

            RuleFor(j => j.Status)
                .IsInEnum()
                .WithMessage("Invalid job status");

            RuleFor(j => j.Priority)
                .IsInEnum()
                .WithMessage("Invalid job priority");

            RuleFor(j => j.Amount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Amount must be greater than or equal to 0")
                .LessThanOrEqualTo(100000)
                .WithMessage("Amount cannot exceed $100,000");

            RuleFor(j => j.EstimatedDeliveryTime)
                .NotEmpty()
                .WithMessage("Estimated delivery time is required")
                .Must(BeValidIsoDateTime)
                .WithMessage("Estimated delivery time must be a valid ISO 8601 date/time string");

            RuleFor(j => j.AssignedDriverId)
                .GreaterThan(0)
                .When(j => j.AssignedDriverId.HasValue)
                .WithMessage("Assigned driver ID must be greater than 0");
        }

        /// <summary>
        /// Validates if the string is a valid ISO 8601 date/time.
        /// </summary>
        /// <param name="dateTimeString">The date/time string to validate.</param>
        /// <returns>True if valid, false otherwise.</returns>
        private static bool BeValidIsoDateTime(string dateTimeString)
        {
            return DateTime.TryParse(dateTimeString, out _);
        }
    }
}
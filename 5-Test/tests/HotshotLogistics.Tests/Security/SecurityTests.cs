// <copyright file="SecurityTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using HotshotLogistics.Api.Services;
using HotshotLogistics.Api.Validation;
using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HotshotLogistics.Tests.Security
{
    /// <summary>
    /// Security-focused tests for the application.
    /// </summary>
    public class SecurityTests
    {
        /// <summary>
        /// Tests that driver validation rejects malicious input.
        /// </summary>
        [Fact]
        public void DriverValidator_RejectsMaliciousInput()
        {
            var validator = new DriverRequestValidator();

            // Test SQL injection attempt
            var maliciousDriver = new Driver
            {
                FirstName = "'; DROP TABLE Drivers; --",
                LastName = "Test",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                LicenseNumber = "ABC123",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(1)
            };

            var result = validator.Validate(maliciousDriver);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Driver.FirstName));
        }

        /// <summary>
        /// Tests that driver validation rejects XSS attempts.
        /// </summary>
        [Fact]
        public void DriverValidator_RejectsXSSInput()
        {
            var validator = new DriverRequestValidator();

            var xssDriver = new Driver
            {
                FirstName = "<script>alert('xss')</script>",
                LastName = "Test",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                LicenseNumber = "ABC123",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(1)
            };

            var result = validator.Validate(xssDriver);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Driver.FirstName));
        }

        /// <summary>
        /// Tests that email validation works correctly.
        /// </summary>
        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("")]
        public void DriverValidator_RejectsInvalidEmails(string email)
        {
            var validator = new DriverRequestValidator();
            var driver = new Driver
            {
                FirstName = "Test",
                LastName = "User",
                Email = email,
                PhoneNumber = "1234567890",
                LicenseNumber = "ABC123",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(1)
            };

            var result = validator.Validate(driver);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Driver.Email));
        }

        /// <summary>
        /// Tests that license expiry validation prevents expired licenses.
        /// </summary>
        [Fact]
        public void DriverValidator_RejectsExpiredLicense()
        {
            var validator = new DriverRequestValidator();
            var driver = new Driver
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                LicenseNumber = "ABC123",
                LicenseExpiryDate = DateTime.UtcNow.AddDays(-1) // Expired yesterday
            };

            var result = validator.Validate(driver);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Driver.LicenseExpiryDate));
        }

        /// <summary>
        /// Tests that job validation rejects malicious input.
        /// </summary>
        [Fact]
        public void JobValidator_RejectsMaliciousInput()
        {
            var validator = new JobRequestValidator();
            var maliciousJob = new Job
            {
                Title = "<script>alert('xss')</script>",
                PickupAddress = "123 Test St",
                DropoffAddress = "456 Test Ave",
                Status = JobStatus.Pending,
                Priority = JobPriority.Medium,
                Amount = 100.00m,
                EstimatedDeliveryTime = DateTime.UtcNow.AddDays(1).ToString("O")
            };

            var result = validator.Validate(maliciousJob);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Job.Title));
        }

        /// <summary>
        /// Tests that job validation enforces amount limits.
        /// </summary>
        [Theory]
        [InlineData(-100.00)]
        [InlineData(150000.00)]
        public void JobValidator_EnforcesAmountLimits(decimal amount)
        {
            var validator = new JobRequestValidator();
            var job = new Job
            {
                Title = "Test Job",
                PickupAddress = "123 Test St",
                DropoffAddress = "456 Test Ave",
                Status = JobStatus.Pending,
                Priority = JobPriority.Medium,
                Amount = amount,
                EstimatedDeliveryTime = DateTime.UtcNow.AddDays(1).ToString("O")
            };

            var result = validator.Validate(job);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Job.Amount));
        }

        /// <summary>
        /// Tests that rate limiting works correctly.
        /// </summary>
        [Fact]
        public void RateLimiting_EnforcesLimits()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var rateLimitingService = new RateLimitingService(cache, TimeSpan.FromMinutes(1), 3);
            const string clientId = "test-client";

            // First 3 requests should be allowed
            Assert.True(rateLimitingService.IsRequestAllowed(clientId));
            Assert.True(rateLimitingService.IsRequestAllowed(clientId));
            Assert.True(rateLimitingService.IsRequestAllowed(clientId));

            // 4th request should be denied
            Assert.False(rateLimitingService.IsRequestAllowed(clientId));
        }

        /// <summary>
        /// Tests that rate limiting tracks remaining requests correctly.
        /// </summary>
        [Fact]
        public void RateLimiting_TracksRemainingRequests()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var rateLimitingService = new RateLimitingService(cache, TimeSpan.FromMinutes(1), 5);
            const string clientId = "test-client";

            // Initially should have full allowance
            Assert.Equal(5, rateLimitingService.GetRemainingRequests(clientId));

            // After one request, should have 4 remaining
            rateLimitingService.IsRequestAllowed(clientId);
            Assert.Equal(4, rateLimitingService.GetRemainingRequests(clientId));
        }

        /// <summary>
        /// Tests that JWT token validation works correctly.
        /// </summary>
        [Fact]
        public void AuthenticationService_ValidatesTokensCorrectly()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_SECRET_KEY"] = "ThisIsASecretKeyForTesting1234567890",
                    ["JWT_ISSUER"] = "test-issuer",
                    ["JWT_AUDIENCE"] = "test-audience"
                })
                .Build();

            var authService = new AuthenticationService(configuration);

            // Generate a token
            var token = authService.GenerateToken("test-user", "admin");
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Validate the token
            var principal = authService.ValidateToken(token);
            Assert.NotNull(principal);
            
            // Check for the user ID in various claim formats
            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                            ?? principal.FindFirst("sub") 
                            ?? principal.FindFirst("nameid");
            
            Assert.NotNull(userIdClaim);
            Assert.Equal("test-user", userIdClaim.Value);
        }

        /// <summary>
        /// Tests that authentication service rejects invalid tokens.
        /// </summary>
        [Fact]
        public void AuthenticationService_RejectsInvalidTokens()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_SECRET_KEY"] = "ThisIsASecretKeyForTesting1234567890",
                    ["JWT_ISSUER"] = "test-issuer",
                    ["JWT_AUDIENCE"] = "test-audience"
                })
                .Build();

            var authService = new AuthenticationService(configuration);

            // Test with invalid token
            var invalidToken = "invalid.token.here";
            var principal = authService.ValidateToken(invalidToken);
            Assert.Null(principal);
        }

        /// <summary>
        /// Tests input length validation.
        /// </summary>
        [Fact]
        public void Validation_EnforcesLengthLimits()
        {
            var validator = new DriverRequestValidator();
            var longStringDriver = new Driver
            {
                FirstName = new string('A', 100), // Exceeds 50 character limit
                LastName = "Test",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                LicenseNumber = "ABC123",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(1)
            };

            var result = validator.Validate(longStringDriver);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(Driver.FirstName));
        }
    }
}
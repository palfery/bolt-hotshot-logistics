// <copyright file="RateLimitingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.Extensions.Caching.Memory;

namespace HotshotLogistics.Api.Services
{
    /// <summary>
    /// Service for implementing rate limiting functionality.
    /// </summary>
    public class RateLimitingService
    {
        private readonly IMemoryCache cache;
        private readonly TimeSpan windowSize;
        private readonly int maxRequests;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitingService"/> class.
        /// </summary>
        /// <param name="cache">The memory cache.</param>
        /// <param name="windowSize">The time window for rate limiting (default: 1 minute).</param>
        /// <param name="maxRequests">The maximum number of requests per window (default: 100).</param>
        public RateLimitingService(IMemoryCache cache, TimeSpan? windowSize = null, int maxRequests = 100)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.windowSize = windowSize ?? TimeSpan.FromMinutes(1);
            this.maxRequests = maxRequests;
        }

        /// <summary>
        /// Checks if a request is allowed based on rate limiting rules.
        /// </summary>
        /// <param name="clientId">The client identifier (IP address, user ID, etc.).</param>
        /// <returns>True if the request is allowed, false if rate limited.</returns>
        public bool IsRequestAllowed(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return false;
            }

            var key = $"rate_limit_{clientId}";
            var now = DateTime.UtcNow;

            if (this.cache.TryGetValue(key, out List<DateTime>? requestTimes))
            {
                // Remove expired entries
                requestTimes = requestTimes!.Where(time => now - time < this.windowSize).ToList();
                
                if (requestTimes.Count >= this.maxRequests)
                {
                    // Update cache with cleaned list
                    this.cache.Set(key, requestTimes, this.windowSize);
                    return false;
                }

                // Add current request time
                requestTimes.Add(now);
                this.cache.Set(key, requestTimes, this.windowSize);
            }
            else
            {
                // First request from this client
                this.cache.Set(key, new List<DateTime> { now }, this.windowSize);
            }

            return true;
        }

        /// <summary>
        /// Gets the remaining requests for a client.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The number of remaining requests in the current window.</returns>
        public int GetRemainingRequests(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return 0;
            }

            var key = $"rate_limit_{clientId}";
            var now = DateTime.UtcNow;

            if (this.cache.TryGetValue(key, out List<DateTime>? requestTimes))
            {
                // Remove expired entries
                var validRequests = requestTimes!.Where(time => now - time < this.windowSize).Count();
                return Math.Max(0, this.maxRequests - validRequests);
            }

            return this.maxRequests;
        }

        /// <summary>
        /// Gets the time until the rate limit window resets.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <returns>The time until reset, or null if no rate limiting is active.</returns>
        public TimeSpan? GetTimeUntilReset(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return null;
            }

            var key = $"rate_limit_{clientId}";
            var now = DateTime.UtcNow;

            if (this.cache.TryGetValue(key, out List<DateTime>? requestTimes))
            {
                var oldestValidRequest = requestTimes!.Where(time => now - time < this.windowSize).Min();
                return this.windowSize - (now - oldestValidRequest);
            }

            return null;
        }
    }
}
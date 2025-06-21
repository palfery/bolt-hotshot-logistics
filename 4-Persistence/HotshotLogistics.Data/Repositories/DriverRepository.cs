// <copyright file="DriverRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data.Repositories
{
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Contracts.Repositories;
    using HotshotLogistics.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository for managing Driver entities.
    /// </summary>
    internal class DriverRepository : IDriverRepository
    {
        private readonly HotshotDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public DriverRepository(HotshotDbContext dbContext)
        {
            context = dbContext;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IDriver>> GetDriversAsync(CancellationToken cancellationToken = default)
        {
            return await context.Drivers.Cast<IDriver>().ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<IDriver?> GetDriverByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Drivers.FindAsync(new object[] { id }, cancellationToken) as IDriver;
        }

        /// <inheritdoc/>
        public async Task<IDriver> CreateDriverAsync(IDriver driver, CancellationToken cancellationToken = default)
        {
            var domainDriver = (Driver)driver;
            domainDriver.CreatedAt = DateTime.UtcNow;
            await context.Drivers.AddAsync(domainDriver, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return domainDriver;
        }

        /// <inheritdoc/>
        public async Task<IDriver> UpdateDriverAsync(IDriver driver, CancellationToken cancellationToken = default)
        {
            var domainDriver = (Driver)driver;
            domainDriver.UpdatedAt = DateTime.UtcNow;
            context.Drivers.Update(domainDriver);
            await context.SaveChangesAsync(cancellationToken);
            return domainDriver;
        }

        /// <inheritdoc/>
        public async Task DeleteDriverAsync(int id, CancellationToken cancellationToken = default)
        {
            var driver = await context.Drivers.FindAsync(new object[] { id }, cancellationToken);
            if (driver != null)
            {
                context.Drivers.Remove(driver);
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

// <copyright file="DriverRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Contracts.Repositories;
    using HotshotLogistics.Domain.Models;
    using Microsoft.EntityFrameworkCore;

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
            this.context = dbContext;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IDriver>> GetDriversAsync()
        {
            return await this.context.Drivers.Cast<IDriver>().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IDriver?> GetDriverByIdAsync(int id)
        {
            return await this.context.Drivers.FindAsync(id) as IDriver;
        }

        /// <inheritdoc/>
        public async Task<IDriver> CreateDriverAsync(IDriver driver)
        {
            var domainDriver = (Driver)driver;
            domainDriver.CreatedAt = DateTime.UtcNow;
            await this.context.Drivers.AddAsync(domainDriver);
            await this.context.SaveChangesAsync();
            return domainDriver;
        }

        /// <inheritdoc/>
        public async Task<IDriver> UpdateDriverAsync(IDriver driver)
        {
            var domainDriver = (Driver)driver;
            domainDriver.UpdatedAt = DateTime.UtcNow;
            this.context.Drivers.Update(domainDriver);
            await this.context.SaveChangesAsync();
            return domainDriver;
        }

        /// <inheritdoc/>
        public async Task DeleteDriverAsync(int id)
        {
            var driver = await this.context.Drivers.FindAsync(id);
            if (driver != null)
            {
                this.context.Drivers.Remove(driver);
                await this.context.SaveChangesAsync();
            }
        }
    }
}

// <copyright file="HotshotDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data
{
    using HotshotLogistics.Contracts.Models;
    using HotshotLogistics.Data.Configurations;
    using HotshotLogistics.Domain.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Database context for the Hotshot Logistics application.
    /// </summary>
    public class HotshotDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotshotDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public HotshotDbContext(DbContextOptions<HotshotDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the drivers in the system.
        /// </summary>
        public DbSet<Driver> Drivers { get; set; }

        /// <summary>
        /// Gets or sets the jobs in the system.
        /// </summary>
        public DbSet<Job> Jobs { get; set; }

        /// <summary>
        /// Gets or sets the job assignments in the system.
        /// </summary>
        public DbSet<JobAssignment> JobAssignments { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DriverConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfiguration());
            modelBuilder.ApplyConfiguration(new JobAssignmentConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
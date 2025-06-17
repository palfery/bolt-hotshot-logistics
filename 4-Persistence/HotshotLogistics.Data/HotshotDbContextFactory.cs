// <copyright file="HotshotDbContextFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace HotshotLogistics.Data
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    /// <summary>
    /// Factory for creating HotshotDbContext instances at design time.
    /// </summary>
    public class HotshotDbContextFactory : IDesignTimeDbContextFactory<HotshotDbContext>
    {
        /// <inheritdoc/>
        public HotshotDbContext CreateDbContext(string[] args)
        {
            var dbUser = Environment.GetEnvironmentVariable("HSL_DBUser") ?? "root";
            var dbPassword = Environment.GetEnvironmentVariable("HSL_DBPassword") ?? string.Empty;
            var dbPort = Environment.GetEnvironmentVariable("HSL_DBPort") ?? "3306";
            var connectionString = $"server=localhost;port={dbPort};database=hotshot_logistics;user={dbUser};password={dbPassword}";
            var optionsBuilder = new DbContextOptionsBuilder<HotshotDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            return new HotshotDbContext(optionsBuilder.Options);
        }
    }
}
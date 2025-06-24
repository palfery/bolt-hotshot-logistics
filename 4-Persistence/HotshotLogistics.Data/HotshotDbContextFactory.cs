// <copyright file="HotshotDbContextFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using HotshotLogistics.Data;
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
        var dbServer = Environment.GetEnvironmentVariable("HSL_DBServer") ?? "localhost";
        var dbName = Environment.GetEnvironmentVariable("HSL_DBName") ?? "hotshot_logistics";
        var dbUser = Environment.GetEnvironmentVariable("HSL_DBUser") ?? "sa";
        var dbPassword = Environment.GetEnvironmentVariable("HSL_DBPassword") ?? string.Empty;

        var connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=true;";
        var optionsBuilder = new DbContextOptionsBuilder<HotshotDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new HotshotDbContext(optionsBuilder.Options);
    }
}

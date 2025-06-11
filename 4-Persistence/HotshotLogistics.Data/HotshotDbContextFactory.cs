using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace HotshotLogistics.Data;

public class HotshotDbContextFactory : IDesignTimeDbContextFactory<HotshotDbContext>
{
    public HotshotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HotshotDbContext>();
        var dbUser = Environment.GetEnvironmentVariable("HSL_DBUser") ?? "root";
        var dbPassword = Environment.GetEnvironmentVariable("HSL_DBPassword") ?? string.Empty;
        var connectionString = $"server=localhost;port=3306;database=hotshot_logistics;user={dbUser};password={dbPassword}";
        
        optionsBuilder.UseMySql(connectionString, 
            ServerVersion.AutoDetect(connectionString),
            options => options.EnableRetryOnFailure());

        return new HotshotDbContext(optionsBuilder.Options);
    }
} 
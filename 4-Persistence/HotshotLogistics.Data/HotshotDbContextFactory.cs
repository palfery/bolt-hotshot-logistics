using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace HotshotLogistics.Data;

public class HotshotDbContextFactory : IDesignTimeDbContextFactory<HotshotDbContext>
{
    public HotshotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HotshotDbContext>();
        var connectionString = "server=localhost;port=3307;database=hotshot_logistics;user=hotshot_user;password=hotshot_password";
        
        optionsBuilder.UseMySql(connectionString, 
            ServerVersion.AutoDetect(connectionString),
            options => options.EnableRetryOnFailure());

        return new HotshotDbContext(optionsBuilder.Options);
    }
} 
using HotshotLogistics.Core.Models;
using HotshotLogistics.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HotshotLogistics.Data;

public class HotshotDbContext : DbContext
{
    public HotshotDbContext(DbContextOptions<HotshotDbContext> options) : base(options)
    {
    }

    public DbSet<Driver> Drivers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DriverConfiguration());
    }
} 
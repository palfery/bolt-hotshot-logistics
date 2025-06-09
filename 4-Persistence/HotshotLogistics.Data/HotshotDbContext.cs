using HotshotLogistics.Domain.Models;
using HotshotLogistics.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Data;

public class HotshotDbContext : DbContext
{
    public HotshotDbContext(DbContextOptions<HotshotDbContext> options) : base(options)
    {
    }

    public DbSet<IDriver> Drivers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new DriverConfiguration());
    }
} 
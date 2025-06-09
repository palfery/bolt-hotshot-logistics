using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Data;

namespace HotshotLogistics.Data.Repositories
{
      
    internal class DriverRepository
    {
        private readonly HotshotDbContext _dbContext;

        public DriverRepository(HotshotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IDriver>> GetDriversAsync()
        {
            return await _dbContext.Drivers.ToListAsync();
        }

        public async Task<IDriver?> GetDriverByIdAsync(int id)
        {
            return await _dbContext.Drivers.FindAsync(id);
        }


        public async Task<IDriver> CreateDriverAsync(IDriver driver)
        {
            driver.CreatedAt = System.DateTime.UtcNow;
            await _dbContext.Drivers.AddAsync(driver);
            await _dbContext.SaveChangesAsync();
            return driver;
        }
    }
}

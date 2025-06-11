using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Contracts.Repositories;
using HotshotLogistics.Data;
using HotshotLogistics.Domain.Models;

namespace HotshotLogistics.Data.Repositories
{

    public class DriverRepository : IDriverRepository
    {
        private readonly HotshotDbContext _dbContext;

        public DriverRepository(HotshotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<IDriver>> GetDriversAsync()
        {
            return await _dbContext.Drivers.Cast<IDriver>().ToListAsync();
        }

        public async Task<IDriver?> GetDriverByIdAsync(int id)
        {
            return await _dbContext.Drivers.FindAsync(id) as IDriver;
        }


        public async Task<IDriver> CreateDriverAsync(IDriver driver)
        {
            driver.CreatedAt = System.DateTime.UtcNow;
            await _dbContext.Drivers.AddAsync((Driver)driver);
            await _dbContext.SaveChangesAsync();
            return driver;
        }

        public async Task DeleteDriverAsync(int id)
        {
            var entity = await _dbContext.Drivers.FindAsync(id);
            if (entity != null)
            {
                _dbContext.Drivers.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IDriver> UpdateDriverAsync(IDriver driver)
        {
            _dbContext.Drivers.Update((Driver)driver);
            await _dbContext.SaveChangesAsync();
            return driver;
        }
    }
}

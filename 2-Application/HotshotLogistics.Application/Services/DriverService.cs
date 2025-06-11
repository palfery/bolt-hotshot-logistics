using HotshotLogistics.Contracts.Models;
using HotshotLogistics.Contracts.Services;
using HotshotLogistics.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotshotLogistics.Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepo;

        public DriverService(IDriverRepository driverRepo)
        {
            _driverRepo = driverRepo;   
        }
        public Task<IDriver> CreateDriverAsync(IDriver driver)
        {
            return _driverRepo.CreateDriverAsync(driver);
        }

        public Task<IDriver?> GetDriverByIdAsync(int id)
        {
            return _driverRepo.GetDriverByIdAsync(id);
        }

        public Task<IEnumerable<IDriver>> GetDriversAsync()
        {
            return _driverRepo.GetDriversAsync();
        }
    }
}

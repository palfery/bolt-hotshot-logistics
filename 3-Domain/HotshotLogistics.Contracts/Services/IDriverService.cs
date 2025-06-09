using System.Collections.Generic;
using System.Threading.Tasks;
using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Contracts.Services;

public interface IDriverService
{
    Task<IEnumerable<IDriver>> GetDriversAsync();
    Task<IDriver?> GetDriverByIdAsync(int id);
    Task<IDriver> CreateDriverAsync(IDriver driver);
}

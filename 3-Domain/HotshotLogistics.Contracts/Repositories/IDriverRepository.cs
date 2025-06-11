using HotshotLogistics.Contracts.Models;

namespace HotshotLogistics.Contracts.Repositories
{
    public interface IDriverRepository
    {
        Task<IEnumerable<IDriver>> GetDriversAsync();
        Task<IDriver?> GetDriverByIdAsync(int id);
        Task<IDriver> CreateDriverAsync(IDriver driver);
        Task DeleteDriverAsync(int id);
        Task<IDriver> UpdateDriverAsync(IDriver driver);
    }
}

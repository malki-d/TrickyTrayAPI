using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface IGiftRepository
    {
        Task<Gift> AddAsync(CreateGiftDTO gift);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Gift>> GetAllAsync();
        Task<Gift?> GetByIdAsync(int id);
        Task<Gift?> UpdateAsync(UpdateGiftDTO gift, int id);
    }
}
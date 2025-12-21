using TrickyTrayAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TrickyTrayAPI.Services
{
    public interface IGiftService
    {
        Task<Gift> AddAsync(Gift gift);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Gift>> GetAllAsync();
        Task<Gift?> GetByIdAsync(int id);
        Task<Gift> UpdateAsync(Gift gift);
    }
}



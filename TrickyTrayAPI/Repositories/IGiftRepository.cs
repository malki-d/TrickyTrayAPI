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
        Task<IEnumerable<Gift>> GetByCategoryAsync(int categoryId);
        Task<Gift?> GetByIdAsync(int id);
        Task<IEnumerable<Gift>> GetSortedAsync(bool sortByName, bool sortByCategory);
        Task<IEnumerable<Gift>> SearchAsync(string? giftName, string? donorName, int? purchaserCount);
        Task<Gift> UpdateAsync(UpdateGiftDTO gift, int id);
        Task<bool> UpdateWinnerAsync(int giftId, int winnerId);
        Task<IEnumerable<GiftWinnerReportDTO>> GetGiftWinnersReportAsync();
    }
}
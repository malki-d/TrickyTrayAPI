using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Services
{
    public interface IGiftService
    {
        Task<GetGiftDTO> AddAsync(CreateGiftDTO gift);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<byte[]> ExportWinnersReportCsvAsync();
        Task<byte[]> ExportWinnersReportExcelAsync();
        Task<IEnumerable<GetGiftDTO>> GetAllAsync();
        Task<IEnumerable<GetGiftDTO>> GetByCategoryAsync(int categoryId);
        Task<GetGiftDTO?> GetByIdAsync(int id);
        Task<IEnumerable<GiftWinnerReportDTO>> GetGiftWinnersReportAsync();
        Task<IEnumerable<GetGiftDTO>> GetSortedAsync(bool sortByName, bool sortByCategory);
        Task<IEnumerable<GetGiftDTO>> GetSortedGiftsAsync(string sortBy);
        Task<bool> RandomWinnerAsync(int giftId);
        Task<IEnumerable<GetGiftWithWinnerDTO>> RandomWinners();
        Task<IEnumerable<GetGiftDTO>> SearchAsync(string? giftName, string? donorName, int? purchaserCount);
        Task<GetGiftDTO> UpdateAsync(UpdateGiftDTO gift, int id);
    }
}
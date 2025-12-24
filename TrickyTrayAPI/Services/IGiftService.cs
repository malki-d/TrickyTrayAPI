using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Services
{
    public interface IGiftService
    {
        Task<GetGiftDTO> AddAsync(CreateGiftDTO gift);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<GetGiftDTO>> GetAllAsync();
        Task<GetGiftDTO?> GetByIdAsync(int id);
        Task<IEnumerable<GetGiftDTO>> SearchAsync(string? giftName, string? donorName, int? purchaserCount);
        Task<GetGiftDTO> UpdateAsync(UpdateGiftDTO gift, int id);
    }
}
using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Services
{
    public interface IPurchaseItemService
    {
        Task<GetPurchaseItemDTO> AddAsync(CreatePurchaseItemDTO purchaseItem);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<GetPurchaseItemDTO>> GetAllAsync();
        Task<GetPurchaseItemDTO?> GetByIdAsync(int id);
        Task<IEnumerable<GetPurchaseItemDTO>> GetPurchaseItemsForGiftAsync(int giftId);
        Task<IEnumerable<GetPurchaseItemDTO>> GetPurchaseItemsForUserAsync(int userId);
        Task<GetPurchaseItemDTO> UpdateAsync(UpdatePurchaseItemDTO purchaseItem, int id);
    }
}
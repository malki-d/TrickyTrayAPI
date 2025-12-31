using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface IPurchaseItemRepository
    {
        Task<IEnumerable<PurchaseItem>> GetAllAsync();
        Task<PurchaseItem?> GetByIdAsync(int id);
        Task<PurchaseItem> AddAsync(CreatePurchaseItemDTO purchaseItem);
        Task<PurchaseItem> UpdateAsync(UpdatePurchaseItemDTO purchaseItem, int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<PurchaseItem>> GetPurchaseItemsForGiftAsync(int giftId);
        Task<IEnumerable<PurchaseItem>> GetPurchaseItemsForUserAsync(int userId);
    }
}
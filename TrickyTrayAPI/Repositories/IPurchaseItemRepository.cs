using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface IPurchaseItemRepository
    {
        Task<IEnumerable<PurchaseItem>> GetPurchaseItemsForGiftAsync(int giftId);
    }
}
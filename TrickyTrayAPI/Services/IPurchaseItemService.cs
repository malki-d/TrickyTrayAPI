using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Services
{
    public interface IPurchaseItemService
    {
        Task<IEnumerable<GetPurchaseItemDTO>> GetPurchaseItemsForGiftAsync(int giftId);
    }
}
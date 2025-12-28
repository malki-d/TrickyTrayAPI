using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Repositories;

namespace TrickyTrayAPI.Services
{
    public class PurchaseItemService : IPurchaseItemService
    {
        private readonly IPurchaseItemRepository _repository;
        private readonly ILogger<PurchaseItemService> _logger;

        public PurchaseItemService(IPurchaseItemRepository repository, ILogger<PurchaseItemService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<IEnumerable<GetPurchaseItemDTO>> GetPurchaseItemsForGiftAsync(int giftId)
        {
            var items = await _repository.GetPurchaseItemsForGiftAsync(giftId);
            return items.Select(pi => new GetPurchaseItemDTO
            {
                UserName = pi.User.FirstName + " " + pi.User.FirstName,
                GiftName = pi.Gift.Name,
                IsWinner = pi.IsWinner
            });
        }
    }
}

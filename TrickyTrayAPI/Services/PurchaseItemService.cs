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

        public async Task<IEnumerable<GetPurchaseItemDTO>> GetAllAsync()
        {
            try
            {
                var items = await _repository.GetAllAsync();
                _logger.LogInformation("Retrieved all purchase items");

                return items.Select(pi => new GetPurchaseItemDTO
                {
                    Id = pi.Id,
                    GiftId = pi.GiftId,
                    GiftName = pi.Gift?.Name ?? string.Empty,
                    UserId = pi.UserId,
                    UserName = pi.User != null ? pi.User.FirstName + " " + pi.User.LastName : string.Empty,
                    IsWinner = pi.IsWinner
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all purchase items");
                throw;
            }
        }

        public async Task<GetPurchaseItemDTO?> GetByIdAsync(int id)
        {
            try
            {
                var item = await _repository.GetByIdAsync(id);
                if (item == null)
                {
                    _logger.LogWarning("Purchase item not found: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Retrieved purchase item by id: {Id}", id);

                return new GetPurchaseItemDTO
                {
                    Id = item.Id,
                    GiftId = item.GiftId,
                    GiftName = item.Gift?.Name ?? string.Empty,
                    UserId = item.UserId,
                    UserName = item.User != null ? item.User.FirstName + " " + item.User.LastName : string.Empty,
                    IsWinner = item.IsWinner
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase item by id: {Id}", id);
                throw;
            }
        }

        public async Task<GetPurchaseItemDTO> AddAsync(CreatePurchaseItemDTO purchaseItem)
        {
            try
            {
                var newItem = await _repository.AddAsync(purchaseItem);
                _logger.LogInformation("Created purchase item: {Id} for Gift: {GiftId}, User: {UserId}",
                    newItem.Id, newItem.GiftId, newItem.UserId);

                return new GetPurchaseItemDTO
                {
                    Id = newItem.Id,
                    GiftId = newItem.GiftId,
                    GiftName = newItem.Gift?.Name ?? string.Empty,
                    UserId = newItem.UserId,
                    UserName = newItem.User != null ? newItem.User.FirstName + " " + newItem.User.LastName : string.Empty,
                    IsWinner = newItem.IsWinner
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase item for Gift: {GiftId}, User: {UserId}",
                    purchaseItem.GiftId, purchaseItem.UserId);
                throw;
            }
        }

        public async Task<GetPurchaseItemDTO> UpdateAsync(UpdatePurchaseItemDTO purchaseItem, int id)
        {
            try
            {
                var updatedItem = await _repository.UpdateAsync(purchaseItem, id);
                if (updatedItem == null)
                {
                    _logger.LogWarning("Cannot update purchase item - not found: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Updated purchase item: {Id}", id);

                return new GetPurchaseItemDTO
                {
                    Id = updatedItem.Id,
                    GiftId = updatedItem.GiftId,
                    GiftName = updatedItem.Gift?.Name ?? string.Empty,
                    UserId = updatedItem.UserId,
                    UserName = updatedItem.User != null ? updatedItem.User.FirstName + " " + updatedItem.User.LastName : string.Empty,
                    IsWinner = updatedItem.IsWinner
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase item: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var isSucceed = await _repository.DeleteAsync(id);
                if (isSucceed)
                {
                    _logger.LogInformation("Deleted purchase item: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Cannot delete purchase item - not found: {Id}", id);
                }
                return isSucceed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting purchase item: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var exists = await _repository.ExistsAsync(id);
                _logger.LogInformation("Check if purchase item exists: {Id} - {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if purchase item exists: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<GetPurchaseItemDTO>> GetPurchaseItemsForGiftAsync(int giftId)
        {
            try
            {
                var items = await _repository.GetPurchaseItemsForGiftAsync(giftId);
                _logger.LogInformation("Retrieved purchase items for gift: {GiftId}", giftId);

                return items.Select(pi => new GetPurchaseItemDTO
                {
                    Id = pi.Id,
                    GiftId = pi.GiftId,
                    GiftName = pi.Gift?.Name ?? string.Empty,
                    UserId = pi.UserId,
                    UserName = pi.User != null ? pi.User.FirstName + " " + pi.User.LastName : string.Empty,
                    IsWinner = pi.IsWinner
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase items for gift: {GiftId}", giftId);
                throw;
            }
        }

        public async Task<IEnumerable<GetPurchaseItemDTO>> GetPurchaseItemsForUserAsync(int userId)
        {
            try
            {
                var items = await _repository.GetPurchaseItemsForUserAsync(userId);
                _logger.LogInformation("Retrieved purchase items for user: {UserId}", userId);

                return items.Select(pi => new GetPurchaseItemDTO
                {
                    Id = pi.Id,
                    GiftId = pi.GiftId,
                    GiftName = pi.Gift?.Name ?? string.Empty,
                    UserId = pi.UserId,
                    UserName = pi.User != null ? pi.User.FirstName + " " + pi.User.LastName : string.Empty,
                    IsWinner = pi.IsWinner
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase items for user: {UserId}", userId);
                throw;
            }
        }
    }
}

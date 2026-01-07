using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Repositories;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _repository;
        private readonly IGiftRepository _repositorygift;

        private readonly ILogger<CartItemService> _logger;

        public CartItemService(ICartItemRepository repository, ILogger<CartItemService> logger, IGiftRepository repositorygift)
        {
            _repository = repository;
            _logger = logger;
            _repositorygift = repositorygift;
        }

        public async Task<IEnumerable<GetCartItemDTO>> GetAllAsync()
        {
            try
            {
                var cartItems = await _repository.GetAllAsync();
                _logger.LogInformation("Fetched all cart items");

                return cartItems.Select(x => new GetCartItemDTO
                {
                    UserName = x.User.FirstName + " " + x.User.LastName,
                    GiftName = x.Gift.Name,
                    Quantity = x.Quantity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cart items");
                throw;
            }
        }

        public async Task<GetCartItemDTO?> GetByIdAsync(int id)
        {
            try
            {
                var cartItem = await _repository.GetByIdAsync(id);
                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item with id {Id} not found", id);
                    return null;
                }

                return new GetCartItemDTO
                {
                    UserName = cartItem.User.FirstName + " " + cartItem.User.LastName,
                    GiftName = cartItem.Gift.Name,
                    Quantity = cartItem.Quantity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get cart item by id {Id}", id);
                throw;
            }
        }

        public async Task<CartItem> CreateAsync(CreateCartItemDTO dto)
        {
            try
            {
                var iswinner = await _repositorygift.GetByIdAsync(dto.GiftId);
                if(iswinner.WinnerId!=0)
                {
                    _logger.LogError("this gift was random " + dto.GiftId);
                    return null;
                }
                    
                var created = await _repository.AddAsync(dto);
                _logger.LogInformation("Created cart item with id {Id}", created.Id);
                return created;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create cart item");
                throw;
            }
        }

        public async Task<UpdateCartItemDTO> UpdateAsync(int id, UpdateCartItemDTO dto)
        {
            try
            {
                var result = await _repository.UpdateAsync(id, dto);
                if (result)
                    _logger.LogInformation("Updated cart item with id {Id}", id);
                else
                    _logger.LogWarning("Cart item with id {Id} not found for update", id);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cart item with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                if (result)
                    _logger.LogInformation("Deleted cart item with id {Id}", id);
                else
                    _logger.LogWarning("Cart item with id {Id} not found for deletion", id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete cart item with id {Id}", id);
                throw;
            }
        }
    }
}
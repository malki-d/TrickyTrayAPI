using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Services
{
    public interface ICartItemService
    {
        Task<CartItem> CreateAsync(CreateCartItemDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<GetCartItemDTO>> GetAllAsync();
        Task<IEnumerable<GetCartItemWithGiftDTO>> GetAllUserCartAsync(int id);
        Task<GetCartItemDTO?> GetByIdAsync(int id);
        Task<UpdateCartItemDTO> UpdateAsync(int id, UpdateCartItemDTO dto);
    }
}
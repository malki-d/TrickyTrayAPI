using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface ICartItemRepository
    {
        Task<CartItem> AddAsync(CreateCartItemDTO cartitem);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CartItem>> GetAllAsync();
        Task<IEnumerable<CartItem>> GetAllUserCartAsync(int id);
        Task<CartItem?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateCartItemDTO cartitem);
    }
}
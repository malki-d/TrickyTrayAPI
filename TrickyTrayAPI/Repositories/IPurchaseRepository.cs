using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

public interface IPurchaseRepository
{
    Task AddPurchaseAsync(Purchase purchase);
    Task ClearUserCartAsync(int userId);
    Task<IEnumerable<Purchase>> GetAllAsync();
    Task<List<CartItem>> GetCartItemsByUserIdAsync(int userId);
    Task<PurchaseRevenueDTO> GetTotalRevenueAsync();
    Task SaveAsync();
    Task<IEnumerable<Purchase>> GetAllAsyncByUserId(int Id);

}
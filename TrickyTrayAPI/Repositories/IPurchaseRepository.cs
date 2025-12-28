using TrickyTrayAPI.Models;
using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Repositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase> AddAsync(Purchase purchase);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Purchase>> GetAllAsync();
        Task<Purchase?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Purchase purchase);
        Task<PurchaseRevenueDTO> GetTotalRevenueAsync();
    }
}
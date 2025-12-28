using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Services
{
    public interface IPurchaseService
    {
        Task<GetPurchaseDTO> AddAsync(CreatePurchaseDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<GetPurchaseDTO>> GetAllAsync();
        Task<GetPurchaseDTO?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, CreatePurchaseDTO dto);
        Task<PurchaseRevenueDTO> GetTotalRevenueAsync();
    }
}
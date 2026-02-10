using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

public interface IPurchaseService
{
    Task<IEnumerable<UserPurchaseDto>> GetAllAsync();
    Task<PurchaseRevenueDTO> GetTotalRevenueAsync();
    Task<IEnumerable<UserPurchaseDto>> GetUserPurchasesLogic(int userId);
    Task<Purchase> ProcessPurchaseAsync(int userId);
}